using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

// generic repository: the shared CRUD code is written once and works for every entity type T.
// T must derive from BaseEntity, so only our models (Game, SportField, Account) can be used here.
// every method talks to the database asynchronously, so no request thread is blocked while waiting.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    // every BaseEntity has an int Id, so the key type is known
    public virtual async Task<T?> GetByIdAsync(int id) =>
        await _dbContext.Set<T>().FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> GetAllAsync() =>
        await _dbContext.Set<T>().ToListAsync();

    // generic method: TKey is the type of the property we sort by (DateTime, string, int, enum...),
    // the compiler infers it from the lambda, e.g. GetAllOrderedByAsync(g => g.StartTime).
    // TKey must be IComparable, so unsortable things like lists or whole objects don't compile.
    public virtual async Task<IReadOnlyList<T>> GetAllOrderedByAsync<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable =>
        await _dbContext.Set<T>().OrderBy(keySelector).ToListAsync();

    public virtual async Task UpdateAsync(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is null)
            return false;

        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
