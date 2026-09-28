using Backend.Data;

namespace SportMatch.API.Utils
{
    public class GameMembershipService
    {
        // game is almost full when 80% of spots are taken
        private const double AlmostFullRatio = 0.8;

        // same static game list that GamesController uses
        private readonly GameStore _store = new();

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

            // can't join a game that already started
            if (game.StartTime <= DateTime.UtcNow)
                return MembershipResult.GameStarted;

            // user can't join the same game twice
            if (game.JoinedUsernames.Contains(username))
                return MembershipResult.AlreadyJoined;

            // no free spots left
            if (GetFreeSpots(game) <= 0)
                return MembershipResult.GameFull;

            game.JoinedUsernames.Add(username);
            UpdateStatus(game);
            return MembershipResult.Success;
        }
    }
}
