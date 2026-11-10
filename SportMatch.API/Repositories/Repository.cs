using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;

namespace SportMatch.API.Repositories;

// generic repository: the shared CRUD code is written once and works for any entity type T
public class Repository<T> where T : class
{
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<T> _set;

    public Repository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _set = dbContext.Set<T>(); // the table for T, like Games or SportFields
    }

    public T Add(T entity)
    {
        _set.Add(entity);
        _dbContext.SaveChanges();
        return entity;
    }

    // generic method: works with int ids (Game, SportField) and string ids (Account)
    public T? GetById<TKey>(TKey id) where TKey : notnull => _set.Find(id);

    public IReadOnlyList<T> GetAll() => _set.ToList();

    public void Update(T entity)
    {
        _set.Update(entity);
        _dbContext.SaveChanges();
    }

    public bool Delete<TKey>(TKey id) where TKey : notnull
    {
        var entity = GetById(id);
        if (entity is null)
            return false;

        _set.Remove(entity);
        _dbContext.SaveChanges();
        return true;
    }
}