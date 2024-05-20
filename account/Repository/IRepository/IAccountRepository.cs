using account.Models;
using System.Linq.Expressions;

namespace account.Repository.IRepository
{
	public interface IAccountRepository : IRepository<Account> 
	{
		Task<IEnumerable<Account>> GetAccountsByUserIdAsync(string userId, Expression<Func<Account, bool>>? filter = null, string? includeProperties = null);
		Task<Account> GetAccountByIdAsync(int accountId);
		Task<Account> UpdateAsync(Account account);
	}
}
