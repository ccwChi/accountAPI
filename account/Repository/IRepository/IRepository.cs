using System.Linq.Expressions;

namespace account.Repository.IRepository
{
	public interface IRepository<T> where T : class
	{
		Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
			int pageSize = 10, int pageNmber = 1);
		Task<T> GetAsync(Expression<Func<T, bool>> filter = null, bool tracked = true);
		Task CreateAsync(T entity);
		Task RemoveAsync(T entity);
		Task SaveAsync();
	}
}
