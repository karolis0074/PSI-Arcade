using SportMatch.API.Models;

namespace SportMatch.API.Repositories
{
    public interface IGameRepository : IRepository<Game>
    {
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(int id);
        Task<Game> AddAsync(Game game);
    }
}
