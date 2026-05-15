using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;


namespace Library_Management.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		[Required]
		[StringLength(100)]
		public string FullName { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		[StringLength(150)]
		public string Email { get; set; } = string.Empty;

		[Required]
		[Phone]
		[StringLength(15)]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required]
		[StringLength(250)]
		public string Address { get; set; } = string.Empty;

		public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

		// Optional Navigation Properties
		[JsonIgnore]
		public Membership? Membership { get; set; }

		[JsonIgnore]
		public ICollection<BorrowRecord>? BorrowRecords { get; set; }
	}
}
