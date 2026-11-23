using System.Collections;
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

    public SportField? GetById<TKey>(TKey id) where TKey : notnull =>
        id is int intId ? Fields.FirstOrDefault(f => f.Id == intId) : null;

    public IReadOnlyList<SportField> GetAll() => Fields;

    public Task<IReadOnlyList<SportField>> GetAllAsync() => Task.FromResult<IReadOnlyList<SportField>>(Fields);

    public Task<SportField?> GetByIdAsync(int id) => Task.FromResult(Fields.FirstOrDefault(f => f.Id == id));

    public void Update(SportField entity) { }

    public bool Delete<TKey>(TKey id) where TKey : notnull
    {
        var field = GetById(id);
        return field is not null && Fields.Remove(field);
    }

    public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null) =>
        Fields;

    public IEnumerator<SportField> GetEnumerator() => Fields.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
