using System.Linq.Expressions;
using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Tests.Fakes;

// fake repository: keeps games in a simple list instead of a database.
// a list answers instantly, so the async methods just wrap the result in Task.FromResult
public class FakeGameRepository : IGameRepository
{
    private readonly List<Game> _games = new();
    private int _nextId = 1;

    // lets a test pretend the database delete failed
    public bool DeleteShouldFail { get; set; }

    // sync helper for arranging tests (e.g. in a constructor, where await is not allowed)
    public Game Add(Game entity)
    {
        if (entity.Id == 0)
            entity.Id = _nextId++;
        _games.Add(entity);
        return entity;
    }

    private Game? Find(int id) => _games.FirstOrDefault(g => g.Id == id);

    public Task<Game> AddAsync(Game entity) => Task.FromResult(Add(entity));

    public Task<Game?> GetByIdAsync(int id) => Task.FromResult(Find(id));

    public Task<IReadOnlyList<Game>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Game>>(_games.ToList());

    public Task<IReadOnlyList<Game>> GetAllOrderedByAsync<TKey>(Expression<Func<Game, TKey>> keySelector)
        where TKey : IComparable =>
        Task.FromResult<IReadOnlyList<Game>>(_games.AsQueryable().OrderBy(keySelector).ToList());

    public Task UpdateAsync(Game entity) => Task.CompletedTask;

    public Task<bool> DeleteAsync(int id)
    {
        if (DeleteShouldFail)
            return Task.FromResult(false);

        var game = Find(id);
        return Task.FromResult(game is not null && _games.Remove(game));
    }
}
