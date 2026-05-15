using Library_Management.Helpers;
using Library_Management.Models;
using Microsoft.Data.SqlClient;

namespace Library_Management.Repositories
{
	public class BookRepository
	{
		private readonly DbHelper _dbHelper;

		public BookRepository(DbHelper dbHelper)
		{
			_dbHelper = dbHelper;
		}

		// Add Book
		public string AddBook(Book book)
		{
			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"INSERT INTO Books
                            (Title, Author, ISBN, Category, TotalCopies, AvailableCopies)
                            VALUES
                            (@Title, @Author, @ISBN, @Category, @TotalCopies, @AvailableCopies)";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@Title", book.Title);
			cmd.Parameters.AddWithValue("@Author", book.Author);
			cmd.Parameters.AddWithValue("@ISBN", book.ISBN);
			cmd.Parameters.AddWithValue("@Category", book.Category);
			cmd.Parameters.AddWithValue("@TotalCopies", book.TotalCopies);
			cmd.Parameters.AddWithValue("@AvailableCopies", book.AvailableCopies);

			

			int rows = cmd.ExecuteNonQuery();

			return rows > 0 ? "Book Added Successfully" : "Failed to Add Book";
		}

		// Get All Books
		public List<Book> GetAllBooks()
		{
			List<Book> books = new List<Book>();

			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Books";

			SqlCommand cmd = new SqlCommand(query, conn);

			//conn.Open();

			SqlDataReader reader = cmd.ExecuteReader();

			while (reader.Read())
			{
				books.Add(MapBook(reader));
			}

			return books;
		}

		// Get Book By Id
		public Book? GetBookById(int bookId)
		{
			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Books WHERE BookId = @BookId";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@BookId", bookId);

			//conn.Open();

			SqlDataReader reader = cmd.ExecuteReader();

			if (reader.Read())
				return MapBook(reader);

			return null;
		}

		// Get Book By ISBN
		public Book? GetBookByISBN(string isbn)
		{
			using SqlConnection conn = _dbHelper.GetConnection();

			string query = "SELECT * FROM Books WHERE ISBN = @ISBN";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@ISBN", isbn);

			//conn.Open();

			SqlDataReader reader = cmd.ExecuteReader();

			if (reader.Read())
				return MapBook(reader);

			return null;
		}

		// Check Availability
		public bool CheckAvailability(int bookId)
		{
			var book = GetBookById(bookId);

			return book != null && book.AvailableCopies > 0;
		}

		// Update Available Copies
		public void UpdateAvailableCopies(int bookId, int change)
		{
			using SqlConnection conn = _dbHelper.GetConnection();

			string query = @"
        UPDATE Books
        SET AvailableCopies = @Change
        WHERE BookId = @BookId";

			SqlCommand cmd = new SqlCommand(query, conn);

			cmd.Parameters.AddWithValue("@Change", change);
			cmd.Parameters.AddWithValue("@BookId", bookId);

			cmd.ExecuteNonQuery();
		}


		// Common Mapper
		private Book MapBook(SqlDataReader reader)
		{
			return new Book
			{
				BookId = Convert.ToInt32(reader["BookId"]),
				Title = reader["Title"].ToString() ?? string.Empty,
				Author = reader["Author"].ToString() ?? string.Empty,
				ISBN = reader["ISBN"].ToString() ?? string.Empty,
				Category = reader["Category"].ToString() ?? string.Empty,
				TotalCopies = Convert.ToInt32(reader["TotalCopies"]),
				AvailableCopies = Convert.ToInt32(reader["AvailableCopies"]),
				AddedDate = Convert.ToDateTime(reader["AddedDate"])
			};
		}
	}
}
