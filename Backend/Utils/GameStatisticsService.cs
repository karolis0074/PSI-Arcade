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
            statistics.MostPopularFieldId = GetMostPopularFieldId(games);

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

        // id of the field with the most games, null when there are no games
        private int? GetMostPopularFieldId(List<Game> games)
        {
            // for every field id, how many games are on that field
            var gamesPerField = new Dictionary<int, int>();

            foreach (var game in games)
            {
                if (gamesPerField.ContainsKey(game.FootballFieldId))
                    gamesPerField[game.FootballFieldId]++;
                else
                    gamesPerField[game.FootballFieldId] = 1;
            }

            int? mostPopularFieldId = null;
            int mostGames = 0;

            foreach (var field in gamesPerField)
            {
                if (field.Value > mostGames)
                {
                    mostPopularFieldId = field.Key;
                    mostGames = field.Value;
                }
            }

            return mostPopularFieldId;
        }
    }
}
