namespace SportMatch.API.Repositories;

public interface IRepository<T> where T : class
{
    T Add(T entity);
    T? GetById<TKey>(TKey id) where TKey : notnull;
    IReadOnlyList<T> GetAll();
    void Update(T entity);
    bool Delete<TKey>(TKey id) where TKey : notnull;
}