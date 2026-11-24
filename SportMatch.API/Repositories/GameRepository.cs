using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

// all async CRUD methods are inherited from Repository<Game>
public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
