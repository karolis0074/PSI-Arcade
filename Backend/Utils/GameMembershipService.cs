using Backend.Data;

namespace SportMatch.API.Utils
{
    public class GameMembershipService
    {
        // game is almost full when 80% of spots are taken
        private const double AlmostFullRatio = 0.8;

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
    }
}
