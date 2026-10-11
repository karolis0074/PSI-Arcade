using Backend.Data;

namespace SportMatch.API.Utils
{
    public class GameMembershipService
    {
        // game is almost full when 80% of spots are taken
        private const double AlmostFullRatio = 0.8;

        // DI — GameStore ateina per konstruktorių
        private readonly GameStore _store;

        public GameMembershipService(GameStore store)
        {
            _store = store;
        }

        public int GetFreeSpots(Game game)
        {
            return game.MaxPlayers - game.JoinedUsernames.Count;
        }

        public void UpdateStatus(Game game)
        {
            int joined = game.JoinedUsernames.Count;

            if (joined >= game.MaxPlayers)
                game.Status = GameStatus.Full;
            else if ((double)joined / game.MaxPlayers >= AlmostFullRatio)
                game.Status = GameStatus.AlmostFull;
            else
                game.Status = GameStatus.Open;
        }

        public MembershipResult Join(int gameId, string username)
        {
            var game = _store.GetById(gameId);

            if (game is null)
                return MembershipResult.GameNotFound;

            if (game.StartTime <= DateTime.UtcNow)
                return MembershipResult.GameStarted;

            if (game.JoinedUsernames.Contains(username))
                return MembershipResult.AlreadyJoined;

            if (GetFreeSpots(game) <= 0)
                return MembershipResult.GameFull;

            game.JoinedUsernames.Add(username);
            UpdateStatus(game);

            _store.Update(game);

            return MembershipResult.Success;
        }

        public MembershipResult Leave(int gameId, string username)
        {
            var game = _store.GetById(gameId);

            if (game is null)
                return MembershipResult.GameNotFound;

            if (game.StartTime <= DateTime.UtcNow)
                return MembershipResult.GameStarted;

            if (!game.JoinedUsernames.Contains(username))
                return MembershipResult.NotJoined;

            game.JoinedUsernames.Remove(username);
            UpdateStatus(game);

            _store.Update(game);

            return MembershipResult.Success;
        }
    }
}