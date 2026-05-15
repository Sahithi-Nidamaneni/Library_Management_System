using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Library_Management.Models
{
	public class Membership
	{
		[Key]
		public int MembershipId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		[StringLength(50)]
		public string MembershipType { get; set; } = string.Empty;

		public DateTime StartDate { get; set; } = DateTime.UtcNow;

		[Required]
		public DateTime EndDate { get; set; }
		


		public bool IsActive { get; set; } = true;

		public string? DocumentUrl { get; set; }

		[JsonIgnore]
		public User? User { get; set; }

		public string? Email { get; set; }
	}
}
