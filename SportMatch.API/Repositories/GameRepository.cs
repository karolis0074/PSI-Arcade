using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

// all CRUD (Add, GetById, GetAll, Update, Delete) is inherited from Repository<Game>
public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}