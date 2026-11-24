using System.Linq.Expressions;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    T Add(T entity);
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    IReadOnlyList<T> GetAllOrderedBy<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable;
    void Update(T entity);
    bool Delete(int id);
}