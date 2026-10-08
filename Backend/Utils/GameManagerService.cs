using Backend.Data;

namespace SportMatch.API.Utils
{
    public class GameManagerService
    {
        private readonly GameStore _store = new();

        public String? GetHostUsername(Game game)
        {
            if (game == null)
                return null;
            return game.JoinedUsernames.First(); // Game has no host field so we use first joined username as the host username (maybe leave like this?)
        }

        public String? GetHostUsername(int id)
        {
            return GetHostUsername(_store.GetById(id));
        }

        public bool IsHost(string username, Game game)
        {
            if (username == null || game == null)
                return false;

            string hostUsername = GetHostUsername(game);

            return hostUsername.Equals(username);
        }

        public bool IsHost(string username, int gameId)
        {
            return IsHost(username, _store.GetById(gameId));
        }
    }
}
