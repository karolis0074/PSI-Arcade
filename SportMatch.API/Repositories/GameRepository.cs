using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<Game>> GetAllAsync() => await _dbContext.Games.ToListAsync();

    public async Task<Game?> GetByIdAsync(int id) => await _dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);

    public async Task<Game> AddAsync(Game game)
    {
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }
}