using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public interface IGameManagerService
    {
        public IReadOnlyList<Game> GetAll();
        public Game? GetById(int id);
        public CreateGameResult Create(string username, CreateGameRequest request);
        public DeleteGameResult Delete(int id, string username);
        public string? GetHostUsername(Game game);
        public string? GetHostUsername(int id);
        public bool IsHost(string username, Game game);
        public bool IsHost(string username, int gameId);
    }
}
