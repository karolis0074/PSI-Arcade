using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services
{
    public class GameManagerService
    {
        private readonly GameRepository _store;

        public GameManagerService(GameRepository store)
        {
            _store = store;
        }

        public String? GetHostUsername(Game game) => game?.CreatedByUsername;
        
        public String? GetHostUsername(int id)
        {
            return GetHostUsername(_store.GetById(id));
        }

        public bool IsHost(string username, Game game)
        {
            if (username == null || game == null)
                return false;

            string? hostUsername = GetHostUsername(game);
            if (hostUsername == null)
                return false;

            return hostUsername.Equals(username);
        }

        public bool IsHost(string username, int gameId)
        {
            return IsHost(username, _store.GetById(gameId));
        }
    }
}
