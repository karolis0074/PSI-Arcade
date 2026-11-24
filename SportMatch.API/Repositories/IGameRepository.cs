using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

// AddAsync, GetByIdAsync, GetAllAsync, UpdateAsync and DeleteAsync come from IRepository<Game>
public interface IGameRepository : IRepository<Game>
{
}
