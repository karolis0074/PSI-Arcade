using System.Linq.Expressions;
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

    public Game? GetById(int id) => _games.FirstOrDefault(g => g.Id == id);

    public IReadOnlyList<Game> GetAll() => _games;

    public IReadOnlyList<Game> GetAllOrderedBy<TKey>(Expression<Func<Game, TKey>> keySelector)
        where TKey : IComparable =>
        _games.AsQueryable().OrderBy(keySelector).ToList();

    public void Update(Game entity) { }

    public bool Delete(int id)
    {
        if (DeleteShouldFail)
            return false;

        var game = GetById(id);
        return game is not null && _games.Remove(game);
    }

    public Task<List<Game>> GetAllAsync() => Task.FromResult(_games.ToList());

    public Task<Game?> GetByIdAsync(int id) => Task.FromResult(GetById(id));

    public Task<Game> AddAsync(Game game) => Task.FromResult(Add(game));
}
