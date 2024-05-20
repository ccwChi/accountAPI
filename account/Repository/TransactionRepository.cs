using account.Data;
using account.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using account.Models;

namespace account.Repository
{
	public class TransactionRepository : Repository<Transaction>, ITransactionRepository
	{
		private readonly ApplicationDbContext _db;
		internal DbSet<Transaction> dbSet;

		public TransactionRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
			this.dbSet = _db.Set<Transaction>();
		}

		public async Task<IEnumerable<Transaction>> GetTransactionsByUserIdAsync(string userId, Expression<Func<Transaction, bool>>? filter = null, string? includeProperties = null)
		{
			IQueryable<Transaction> query = dbSet.Include(t => t.Account).Where(t => t.Account.UserId == userId);

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty);
				}
			}

			return await query.ToListAsync();
		}

		public async Task<Transaction> GetTransactionByIdAsync(int transactionId)
		{
			return await _db.Transactions.Include(t => t.Account).FirstOrDefaultAsync(t => t.TransactionId == transactionId);
		}

		public async Task<Transaction> UpdateAsync(Transaction transaction)
		{
			_db.Transactions.Update(transaction);
			await _db.SaveChangesAsync();
			return transaction;
		}
	}
}

