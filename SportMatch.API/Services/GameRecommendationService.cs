using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Extensions;
using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services;

public class GameRecommendationService : IGameRecommendationService
{
    // how much each criterion counts in the recommendation score
    private const double DistanceWeight = 0.35;
    private const double TimeWeight = 0.20;
    private const double FreeSpotsWeight = 0.15;
    private const double PopularityWeight = 0.15;
    private const double HistoryWeight = 0.15;

    // distance and time at which their part of the score drops to half
    private const double HalfScoreDistanceKm = 5;
    private const double HalfScoreHours = 48;

    private readonly IGameRepository _gameRepository;
    private readonly ISportFieldRepository _sportFieldRepository;
    private readonly IClock _clock;

    public GameRecommendationService(IGameRepository gameRepository, ISportFieldRepository sportFieldRepository, IClock clock)
    {
        _gameRepository = gameRepository;
        _sportFieldRepository = sportFieldRepository;
        _clock = clock;
    }

    // upcoming games that are not full and the user is not in, best score first
    public IReadOnlyList<RecommendedGame> GetRecommended(string username, double? latitude = null, double? longitude = null)
    {
        var now = _clock.UtcNow;
        var games = _gameRepository.GetAll();
        var fields = _sportFieldRepository.GetAll().ToDictionary(field => field.Id);
        bool hasLocation = latitude is not null && longitude is not null;

        var gamesPerField = games
            .GroupBy(game => game.FootballFieldId)
            .ToDictionary(group => group.Key, group => group.Count());
        int mostGamesOnField = gamesPerField.Count == 0 ? 0 : gamesPerField.Values.Max();

        // game types of the fields the user has already played on
        var playedGameTypes = games
            .Where(game => IsPlayer(game, username))
            .Select(game => fields.GetValueOrDefault(game.FootballFieldId)?.GameType)
            .OfType<string>()
            .ToList();

        return games
            .Where(game => game.StartTime > now
                && game.JoinedUsernames.Count < game.MaxPlayers
                && !IsPlayer(game, username))
            .Select(game =>
            {
                var field = fields.GetValueOrDefault(game.FootballFieldId);
                var distanceKm = hasLocation ? GetDistanceKm(field, latitude!.Value, longitude!.Value) : null;

                double weightedScore =
                    TimeWeight * TimeScore(game, now)
                    + FreeSpotsWeight * FreeSpotsScore(game)
                    + PopularityWeight * PopularityScore(game, gamesPerField, mostGamesOnField)
                    + HistoryWeight * HistoryScore(field, playedGameTypes);
                double totalWeight = TimeWeight + FreeSpotsWeight + PopularityWeight + HistoryWeight;

                // without the user's location the distance is left out of the score
                if (hasLocation)
                {
                    weightedScore += DistanceWeight * DistanceScore(distanceKm);
                    totalWeight += DistanceWeight;
                }

                return new RecommendedGame(game, distanceKm, Math.Round(weightedScore / totalWeight, 4));
            })
            .OrderByDescending(recommendation => recommendation.Score)
            .ThenBy(recommendation => recommendation.Game.StartTime)
            .ToList();
    }

    private static bool IsPlayer(Game game, string username) =>
        game.JoinedUsernames.Any(name => name.Equals(username, StringComparison.OrdinalIgnoreCase));

    private static double? GetDistanceKm(SportField? field, double latitude, double longitude)
    {
        if (field is null || !field.TryGetCoordinates(out var fieldLatitude, out var fieldLongitude))
            return null;

        return Math.Round(GeoDistance.Kilometers(latitude, longitude, fieldLatitude, fieldLongitude), 2);
    }

    // a field without coordinates gets 0
    private static double DistanceScore(double? distanceKm) =>
        distanceKm is null ? 0 : 1 / (1 + distanceKm.Value / HalfScoreDistanceKm);

    private static double TimeScore(Game game, DateTime now) =>
        1 / (1 + (game.StartTime - now).TotalHours / HalfScoreHours);

    private static double FreeSpotsScore(Game game) =>
        (double)(game.MaxPlayers - game.JoinedUsernames.Count) / game.MaxPlayers;

    // fields with more games are more popular
    private static double PopularityScore(Game game, Dictionary<int, int> gamesPerField, int mostGamesOnField) =>
        mostGamesOnField == 0 ? 0 : (double)gamesPerField.GetValueOrDefault(game.FootballFieldId) / mostGamesOnField;

    // share of the user's games that were the same sport as this field
    private static double HistoryScore(SportField? field, List<string> playedGameTypes) =>
        field is null || playedGameTypes.Count == 0
            ? 0
            : (double)playedGameTypes.Count(gameType => gameType == field.GameType) / playedGameTypes.Count;
}
