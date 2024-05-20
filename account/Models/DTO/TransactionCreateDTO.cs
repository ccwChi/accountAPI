﻿using System.ComponentModel.DataAnnotations;

namespace account.Models.DTO
{
	public class TransactionCreateDTO
	{
		public int AccountId { get; set; }
		[Required]
		[MaxLength(30)]
		public string Name { get; set; }
		public decimal Amount { get; set; }
		public string TransactionType { get; set; }
		public string Description { get; set; }
		//public DateTime TransactionDate { get; set; }
		//public DateTime CreatedAt { get; set; }
		//public DateTime UpadateAt { get; set; }
	}
}
