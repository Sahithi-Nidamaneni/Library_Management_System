using Library_Management.Helpers;
using Library_Management.Models;
using Microsoft.Data.SqlClient;

namespace Library_Management.Repositories
{
	public class BorrowRepository
	{
		private readonly DbHelper _dbHelper;
		private readonly BookRepository _bookRepository;

		public BorrowRepository(DbHelper dbHelper, BookRepository bookRepository)
		{
			_dbHelper = dbHelper;
			_bookRepository = bookRepository;
		}

		// Borrow Book
		public string BorrowBook(BorrowRecord borrowRecord)
		{
			var book = _bookRepository.GetBookById(borrowRecord.BookId);

			if (book == null)
				return "Book Not Found";

			if (book.AvailableCopies <= 0)
				return "Book Not Available";

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"INSERT INTO BorrowRecords
                            (UserId, BookId, BorrowDate, DueDate, IsReturned, FineAmount)
                            VALUES
                            (@UserId, @BookId, @BorrowDate, @DueDate, @IsReturned, @FineAmount)";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserId", borrowRecord.UserId);
			cmd.Parameters.AddWithValue("@BookId", borrowRecord.BookId);
			cmd.Parameters.AddWithValue("@BorrowDate", borrowRecord.BorrowDate);
			cmd.Parameters.AddWithValue("@DueDate", borrowRecord.DueDate);
			cmd.Parameters.AddWithValue("@IsReturned", false);
			cmd.Parameters.AddWithValue("@FineAmount", 0);

			//conn.Open();

			int rows = cmd.ExecuteNonQuery();

			if (rows > 0)
			{
				_bookRepository.UpdateAvailableCopies(
					borrowRecord.BookId,
					book.AvailableCopies - 1
				);

				return "Book Borrowed Successfully";
			}

			return "Failed to Borrow Book";
		}

		// Return Book
		public string ReturnBook(int borrowRecordId)
		{
			var record = GetBorrowRecordById(borrowRecordId);

			if (record == null)
				return "Borrow Record Not Found";

			if (record.IsReturned)
				return "Book Already Returned";

			DateTime returnDate = DateTime.UtcNow;

			decimal fine = 0;

			if (returnDate > record.DueDate)
			{
				int lateDays = (returnDate - record.DueDate).Days;
				fine = lateDays * 10; // ₹10 per day
			}

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"UPDATE BorrowRecords
                            SET ReturnDate = @ReturnDate,
                                IsReturned = 1,
                                FineAmount = @FineAmount
                            WHERE BorrowRecordId = @BorrowRecordId";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@ReturnDate", returnDate);
			cmd.Parameters.AddWithValue("@FineAmount", fine);
			cmd.Parameters.AddWithValue("@BorrowRecordId", borrowRecordId);

			//conn.Open();

			int rows = cmd.ExecuteNonQuery();

			if (rows > 0)
			{
				var book = _bookRepository.GetBookById(record.BookId);

				if (book != null)
				{
					_bookRepository.UpdateAvailableCopies(
						record.BookId,
						book.AvailableCopies + 1
					);
				}

				return fine > 0
					? $"Book Returned Successfully. Fine: ₹{fine}"
					: "Book Returned Successfully";
			}

			return "Failed to Return Book";
		}

		// Get Borrow Record By Id
		public BorrowRecord? GetBorrowRecordById(int borrowRecordId)
		{
			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM BorrowRecords WHERE BorrowRecordId = @BorrowRecordId";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@BorrowRecordId", borrowRecordId);

			//conn.Open();

			SqlDataReader reader = cmd.ExecuteReader();

			if (reader.Read())
			{
				return MapBorrowRecord(reader);
			}

			return null;
		}
		// ✅ Get All Borrow Records
		public List<BorrowRecord> GetAllBorrowRecords()
		{
			List<BorrowRecord> records = new List<BorrowRecord>();

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM BorrowRecords";

			using SqlCommand cmd = new SqlCommand(query, conn);

			using SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				records.Add(MapBorrowRecord(reader));
			}

			return records;
		}
		// Get Borrow History By User
		public List<BorrowRecord> GetBorrowHistoryByUser(int userId)
		{
			List<BorrowRecord> records = new List<BorrowRecord>();

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM BorrowRecords WHERE UserId = @UserId";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@UserId", userId);

			//conn.Open();

			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				records.Add(MapBorrowRecord(reader));
			}

			return records;
		}

		private BorrowRecord MapBorrowRecord(SqlDataReader reader)
		{
			return new BorrowRecord
			{
				BorrowRecordId = Convert.ToInt32(reader["BorrowRecordId"]),
				UserId = Convert.ToInt32(reader["UserId"]),
				BookId = Convert.ToInt32(reader["BookId"]),
				BorrowDate = Convert.ToDateTime(reader["BorrowDate"]),
				DueDate = Convert.ToDateTime(reader["DueDate"]),
				ReturnDate = reader["ReturnDate"] == DBNull.Value
					? null
					: Convert.ToDateTime(reader["ReturnDate"]),
				IsReturned = Convert.ToBoolean(reader["IsReturned"]),
				FineAmount = Convert.ToDecimal(reader["FineAmount"])
			};
		}
	}
}
