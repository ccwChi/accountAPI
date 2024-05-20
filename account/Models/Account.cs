using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace account.Models
{
	public class Account
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int AccountId { get; set; }
		[Required]
		[MaxLength(30)]
		public string AccountName { get; set; }
		public string AccountType { get; set; }
		public decimal Balance { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdateAt { get; set; }
		public string UserId { get; set; }

		// One-to-Many relationship with Transaction
		public ICollection<Transaction> Transactions { get; set; }
	}
}
