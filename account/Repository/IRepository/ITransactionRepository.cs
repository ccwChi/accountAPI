using account.Models;
using System.Linq.Expressions;

namespace account.Repository.IRepository
{
	public interface ITransactionRepository : IRepository<Transaction>
	{
		Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId, Expression<Func<Transaction, bool>>? filter = null, string? includeProperties = null);
		Task<Transaction> GetTransactionByIdAsync(int transactionId);
		Task<Transaction> UpdateAsync(Transaction transaction);
	}
}
