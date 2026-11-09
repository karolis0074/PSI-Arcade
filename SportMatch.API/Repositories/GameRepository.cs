using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public class GameRepository
{
    private readonly AppDbContext _dbContext;

    public GameRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Game Add(Game game)
    {
        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();
        return game;
    }

    public Game? GetById(int id) =>
        _dbContext.Games.FirstOrDefault(g => g.Id == id);

    public IReadOnlyList<Game> GetAll() => _dbContext.Games.ToList();

    public bool Delete(int id)
    {
        var game = GetById(id);
        if (game is null) return false;

        _dbContext.Games.Remove(game);
        _dbContext.SaveChanges();
        return true;
    }
    public void Update(Game game)
    {
        _dbContext.Games.Update(game);
        _dbContext.SaveChanges();
    }
}
