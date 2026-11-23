using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

// generic repository: the shared CRUD code is written once and works for every entity type T.
// T must derive from BaseEntity, so only our models (Game, SportField, Account) can be used here.
public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _dbContext;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public T Add(T entity)
    {
        _dbContext.Set<T>().Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    // every BaseEntity has an int Id, so the key type is known
    public T? GetById(int id) => _dbContext.Set<T>().Find(id);

    public IReadOnlyList<T> GetAll() => _dbContext.Set<T>().ToList();

    // generic method: TKey is the type of the property we sort by (DateTime, string, int, enum...),
    // the compiler infers it from the lambda, e.g. GetAllOrderedBy(g => g.StartTime).
    // TKey must be IComparable, so unsortable things like lists or whole objects don't compile.
    public IReadOnlyList<T> GetAllOrderedBy<TKey>(Expression<Func<T, TKey>> keySelector)
        where TKey : IComparable =>
        _dbContext.Set<T>().OrderBy(keySelector).ToList();

    public void Update(T entity)
    {
        _dbContext.Set<T>().Update(entity);
        _dbContext.SaveChanges();
    }

    public bool Delete(int id)
    {
        var entity = GetById(id);
        if (entity is null)
            return false;

        _dbContext.Set<T>().Remove(entity);
        _dbContext.SaveChanges();
        return true;
    }
}