using SportMatch.API.Models;

namespace SportMatch.API.Repositories
{
    public interface IGameRepository
    {
        public Game Add(Game game);
        public Game? GetById(int id);
        public IReadOnlyList<Game> GetAll();
        public bool Delete(int id);
        public void Update(Game game);
    }
}
