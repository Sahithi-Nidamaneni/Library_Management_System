using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Library_Management.Models
{
	public class Book
	{
		[Key]
		public int BookId { get; set; }

		[Required]
		[StringLength(200)]
		public string Title { get; set; } = string.Empty;

		[Required]
		[StringLength(150)]
		public string Author { get; set; } = string.Empty;

		[Required]
		[StringLength(50)]
		public string ISBN { get; set; } = string.Empty;

		[Required]
		[StringLength(100)]
		public string Category { get; set; } = string.Empty;

		[Required]
		public int TotalCopies { get; set; }

		[Required]
		public int AvailableCopies { get; set; }

		public DateTime AddedDate { get; set; } = DateTime.UtcNow;

		[JsonIgnore]
		public ICollection<BorrowRecord>? BorrowRecords { get; set; }
	}
}
