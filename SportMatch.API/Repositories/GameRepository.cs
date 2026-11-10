using Microsoft.EntityFrameworkCore;
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

    public List<Game> GetAll() => _dbContext.Games.ToList();

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

    // --- NAUJI ASYNC METODAI 
    public async Task<List<Game>> GetAllAsync() => await _dbContext.Games.ToListAsync();
    
    public async Task<Game?> GetByIdAsync(int id) => await _dbContext.Games.FirstOrDefaultAsync(g => g.Id == id);
    
    public async Task<Game> AddAsync(Game game) 
    { 
        _dbContext.Games.Add(game); 
        await _dbContext.SaveChangesAsync(); 
        return game; 
    }
}