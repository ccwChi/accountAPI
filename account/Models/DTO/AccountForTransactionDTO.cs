using System.ComponentModel.DataAnnotations;

namespace account.Models.DTO
{
	public class AccountForTransactionDTO
	{
		public int AccountId { get; set; }
		public string AccountName { get; set; }
		public string AccountType { get; set; }
		public decimal Balance { get; set; }
		//public DateTime UpdateAt { get; set; }
	}
}
