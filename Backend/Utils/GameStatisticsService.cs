using Backend.Data;

namespace SportMatch.API.Utils
{
    public class GameStatisticsService
    {
        private readonly GameStore _store;

        public GameStatisticsService(GameStore store)
        {
            _store = store;
        }

        public GameStatistics GetOverview()
        {
            var games = _store.GetAll();

            var statistics = new GameStatistics();
            statistics.TotalGames = games.Count;
            statistics.AveragePlayers = GetAveragePlayers(games);

            return statistics;
        }

        // average number of joined players per game
        private double GetAveragePlayers(List<Game> games)
        {
            // no games yet, so there is nothing to divide by
            if (games.Count == 0)
                return 0;

            int totalPlayers = 0;

            foreach (var game in games)
                totalPlayers += game.JoinedUsernames.Count;

            return (double)totalPlayers / games.Count;
        }
    }
}
