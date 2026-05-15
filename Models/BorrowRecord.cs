using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library_Management.Models
{
	public class BorrowRecord
	{

		[Key]
		public int BorrowRecordId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int BookId { get; set; }

		public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

		[Required]
		public DateTime DueDate { get; set; }

		public DateTime? ReturnDate { get; set; }

		public bool IsReturned { get; set; } = false;

		public decimal FineAmount { get; set; } = 0;

		[JsonIgnore]
		public User? User { get; set; }

		[JsonIgnore]
		public Book? Book { get; set; }
	}
}
