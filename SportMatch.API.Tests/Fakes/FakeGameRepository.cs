using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Tests.Fakes;

// fake repository: keeps games in a simple list instead of a database
public class FakeGameRepository : IGameRepository
{
    private readonly List<Game> _games = new();
    private int _nextId = 1;

    // lets a test pretend the database delete failed
    public bool DeleteShouldFail { get; set; }

    public Game Add(Game entity)
    {
        if (entity.Id == 0)
            entity.Id = _nextId++;
        _games.Add(entity);
        return entity;
    }

    public Game? GetById<TKey>(TKey id) where TKey : notnull =>
        id is int intId ? _games.FirstOrDefault(g => g.Id == intId) : null;

    public IReadOnlyList<Game> GetAll() => _games;

    public void Update(Game entity) { }

    public bool Delete<TKey>(TKey id) where TKey : notnull
    {
        if (DeleteShouldFail)
            return false;

        var game = GetById(id);
        return game is not null && _games.Remove(game);
    }
}
