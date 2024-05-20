using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace account.Models
{
	public class Transaction
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int TransactionId { get; set; }
		public int AccountId { get; set; } // Foreign key
		[ForeignKey("AccountId")]
		public Account Account { get; set; }
		[Required]
		[MaxLength(30)]
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public string TransactionType { get; set; }
		public string Description { get; set; }
		//public DateTime TransactionDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpadateAt { get; set; }
	}
}
