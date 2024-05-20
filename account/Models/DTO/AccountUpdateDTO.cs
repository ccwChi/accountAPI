using System.ComponentModel.DataAnnotations;

namespace account.Models.DTO
{
	public class AccountUpdateDTO
	{
		public int AccountId { get; set; }
		[Required]
		[MaxLength(30)]
		public string AccountName { get; set; }
		public string AccountType { get; set; }
		public decimal Balance { get; set; }
		//public DateTime UpdateAt { get; set; }
	}
}
