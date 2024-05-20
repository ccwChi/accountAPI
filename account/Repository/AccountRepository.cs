using account.Data;
using account.Models;
using account.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace account.Repository
{
	public class AccountRepository : Repository<Account>, IAccountRepository
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<Account> dbSet;
		public AccountRepository(ApplicationDbContext db): base(db) 
		{
			_db = db;
			this.dbSet = db.Set<Account>();
		}

		public async Task<IEnumerable<Account>> GetAccountsByUserIdAsync(string userId, Expression<Func<Account, bool>>? filter, string? includeProperties)
		{
			IQueryable<Account> query = dbSet;
			query = query.Where(a => a.UserId == userId);

			// 要進階求得外鍵資料再用
			//if (!string.IsNullOrEmpty(includeProperties))
			//{
			//	foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			//	{
			//		query = query.Include(includeProperty);
			//	}
			//}
			
			return await query.ToListAsync();
		}

		public async Task<Account> GetAccountByIdAsync(int accountId)
		{
			return await _db.Accounts.FindAsync(accountId);
		}

		public async Task<Account> UpdateAsync(Account account)
		{
			account.UpdateAt = DateTime.Now;
			_db.Accounts.Update(account);
			await _db.SaveChangesAsync();
			return account;
		}

	}
}
