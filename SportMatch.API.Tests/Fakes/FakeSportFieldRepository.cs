using System.Collections;
using System.Linq.Expressions;
using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Tests.Fakes;

// fake repository: keeps sport fields in a simple list instead of a database
public class FakeSportFieldRepository : ISportFieldRepository
{
    public List<SportField> Fields { get; } = new();

    public SportField Add(SportField entity)
    {
        Fields.Add(entity);
        return entity;
    }

    public SportField? GetById(int id) => Fields.FirstOrDefault(f => f.Id == id);

    public IReadOnlyList<SportField> GetAll() => Fields;

    public IReadOnlyList<SportField> GetAllOrderedBy<TKey>(Expression<Func<SportField, TKey>> keySelector)
        where TKey : IComparable =>
        Fields.AsQueryable().OrderBy(keySelector).ToList();

    public Task<IReadOnlyList<SportField>> GetAllAsync() => Task.FromResult<IReadOnlyList<SportField>>(Fields);

    public Task<SportField?> GetByIdAsync(int id) => Task.FromResult(GetById(id));

    public void Update(SportField entity) { }

    public bool Delete(int id)
    {
        var field = GetById(id);
        return field is not null && Fields.Remove(field);
    }

    public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null) =>
        Fields;

    public IEnumerator<SportField> GetEnumerator() => Fields.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
