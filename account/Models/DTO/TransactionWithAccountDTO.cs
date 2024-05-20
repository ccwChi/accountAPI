namespace account.Models.DTO
{
	public class TransactionWithAccountDTO
	{
		public int TransactionId { get; set; }
		public int AccountId { get; set; }
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public string TransactionType { get; set; }
		public string Description { get; set; }
		//public DateTime TransactionDate { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
		public AccountForTransactionDTO Account { get; set; }  
	}
}
