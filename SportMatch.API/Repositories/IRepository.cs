using System.Linq.Expressions;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> AddAsync(T entity);
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAllOrderedByAsync<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable;
    Task UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}
