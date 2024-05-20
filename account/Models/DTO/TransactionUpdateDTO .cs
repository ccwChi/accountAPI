using System.ComponentModel.DataAnnotations;

namespace account.Models.DTO
{
	public class TransactionUpdateDTO
	{
		public int TransactionId { get; set; }
		public int AccountId { get; set; }
		[Required]
		[MaxLength(30)]
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public string TransactionType { get; set; }
		public string Description { get; set; }
	}
}
