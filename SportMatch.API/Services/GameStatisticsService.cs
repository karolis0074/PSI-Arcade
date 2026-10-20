using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services;

public class GameStatisticsService
{
    private readonly GameRepository _gameRepo;

    public GameStatisticsService(GameRepository gameRepo)
    {
        _gameRepo = gameRepo;
    }

    public GameStatistics GetOverview()
    {
        var games = _gameRepo.GetAll();

        var statistics = new GameStatistics();
        statistics.TotalGames = games.Count;
        statistics.AveragePlayers = GetAveragePlayers(games);
        statistics.MostPopularFieldId = GetMostPopularFieldId(games);

        return statistics;
    }

    // average number of joined players per game
    private double GetAveragePlayers(IReadOnlyList<Game> games)
    {
        // no games yet, so there is nothing to divide by
        if (games.Count == 0)
            return 0;

        return games.Average(game => game.JoinedUsernames.Count);
    }

    // id of the field with the most games, null when there are no games
    private int? GetMostPopularFieldId(IReadOnlyList<Game> games)
    {
        // group the games by field, the largest group is the most popular field
        return games
            .GroupBy(game => game.FootballFieldId)
            .OrderByDescending(group => group.Count())
            .Select(group => (int?)group.Key)
            .FirstOrDefault();
    }
}
