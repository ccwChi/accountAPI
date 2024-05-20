using account.Data;
using account.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace account.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{

		private readonly ApplicationDbContext _db;
		internal DbSet<T> dbSet;
		public Repository(ApplicationDbContext db)
		{
			_db = db;
			this.dbSet = db.Set<T>();
		}


		public async Task CreateAsync(T entity)
		{
			await dbSet.AddAsync(entity);
			await SaveAsync();
		}

		public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
	int pageSize = 10, int pageNmber = 1)
		{
			IQueryable<T> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}
			if (pageSize > 0)
			{
				query = query.Skip(pageSize * (pageNmber - 1)).Take(pageSize);
			}
			if (includeProperties != null)
			{
				foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(property);
				}
			}


			return await query.ToListAsync();
		}

		public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true)
		{
			IQueryable<T> query = dbSet;
			if (!tracked)
			{
				query = query.AsNoTracking();
			}

			if (filter != null)
			{
				query = query.Where(filter);
			}
			return await query.FirstOrDefaultAsync();

		}

		public async Task RemoveAsync(T entity)
		{
			dbSet.Remove(entity);
			await SaveAsync();
		}


		public async Task SaveAsync()
		{
			await _db.SaveChangesAsync();
		}
	}
}
