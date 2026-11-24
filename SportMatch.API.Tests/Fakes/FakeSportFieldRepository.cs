using System.Collections;
using System.Linq.Expressions;
using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Tests.Fakes;

// fake repository: keeps sport fields in a simple list instead of a database
public class FakeSportFieldRepository : ISportFieldRepository
{
    public List<SportField> Fields { get; } = new();

    // sync helper for arranging tests (e.g. in a constructor, where await is not allowed)
    public SportField Add(SportField entity)
    {
        Fields.Add(entity);
        return entity;
    }

    private SportField? Find(int id) => Fields.FirstOrDefault(f => f.Id == id);

    public Task SeedIfEmptyAsync() => Task.CompletedTask;

    public Task<SportField> AddAsync(SportField entity) => Task.FromResult(Add(entity));

    public Task<SportField?> GetByIdAsync(int id) => Task.FromResult(Find(id));

    public Task<IReadOnlyList<SportField>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<SportField>>(Fields);

    public Task<IReadOnlyList<SportField>> GetAllOrderedByAsync<TKey>(Expression<Func<SportField, TKey>> keySelector)
        where TKey : IComparable =>
        Task.FromResult<IReadOnlyList<SportField>>(Fields.AsQueryable().OrderBy(keySelector).ToList());

    public Task UpdateAsync(SportField entity) => Task.CompletedTask;

    public Task<bool> DeleteAsync(int id)
    {
        var field = Find(id);
        return Task.FromResult(field is not null && Fields.Remove(field));
    }

    public Task<IReadOnlyList<SportField>> SearchAsync(string? city = null, string? gameType = null, string? fieldType = null) =>
        Task.FromResult<IReadOnlyList<SportField>>(Fields);

    public IEnumerator<SportField> GetEnumerator() => Fields.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
