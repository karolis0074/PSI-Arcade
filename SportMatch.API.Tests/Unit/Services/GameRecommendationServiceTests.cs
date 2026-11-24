using SportMatch.API.Models;
using SportMatch.API.Services;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Services;

// Unit tests: GameRecommendationService gets FAKE repositories and a FAKE clock,
// so every game, field and "now" is set up by the test itself.
public class GameRecommendationServiceTests
{
    private const string User = "jonas";

    // the user is in Vilnius center
    private const double UserLatitude = 54.68;
    private const double UserLongitude = 25.28;

    // about 1.1 km and about 92 km from the user
    private const string NearAddress = "54.69, 25.28";
    private const string FarAddress = "54.90, 23.89";

    private readonly FakeGameRepository _gameRepository = new();
    private readonly FakeSportFieldRepository _fieldRepository = new();
    private readonly FakeClock _clock = new();
    private readonly GameRecommendationService _service;

    public GameRecommendationServiceTests()
    {
        _service = new GameRecommendationService(_gameRepository, _fieldRepository, _clock);
    }

    private void AddField(int id, string address = NearAddress, string gameType = "Football")
    {
        _fieldRepository.Add(new SportField { Id = id, Name = $"Field {id}", Address = address, GameType = gameType, Capacity = 10 });
    }

    // puts a game straight into the fake repository, the host is always in the game
    private Game AddGame(int fieldId, double hoursFromNow = 24, int maxPlayers = 10, string[]? players = null)
    {
        var game = new Game
        {
            FootballFieldId = fieldId,
            CreatedByUsername = "host",
            StartTime = _clock.UtcNow.AddHours(hoursFromNow),
            MaxPlayers = maxPlayers
        };
        game.JoinedUsernames.Add("host");
        game.JoinedUsernames.AddRange(players ?? Array.Empty<string>());

        return _gameRepository.Add(game);
    }

    // ---------- which games are recommended ----------

    [Fact]
    public void GetRecommended_NoGames_ReturnsEmptyList()
    {
        var result = _service.GetRecommended(User);

        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommended_GameAlreadyStarted_IsNotRecommended()
    {
        AddField(1);
        AddGame(1, hoursFromNow: -1);

        var result = _service.GetRecommended(User);

        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommended_GameIsFull_IsNotRecommended()
    {
        AddField(1);
        AddGame(1, maxPlayers: 2, players: new[] { "petras" });

        var result = _service.GetRecommended(User);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData("jonas")]
    [InlineData("Jonas")]
    public void GetRecommended_UserAlreadyInGame_IsNotRecommended(string joinedAs)
    {
        AddField(1);
        AddGame(1, players: new[] { joinedAs });

        var result = _service.GetRecommended(User);

        Assert.Empty(result);
    }

    [Fact]
    public void GetRecommended_UpcomingGameWithFreeSpots_IsRecommended()
    {
        AddField(1);
        var game = AddGame(1);

        var result = _service.GetRecommended(User);

        Assert.Equal(game.Id, Assert.Single(result).Game.Id);
    }

    // ---------- distance ----------

    [Fact]
    public void GetRecommended_WithLocation_CloserFieldComesFirst()
    {
        AddField(1, FarAddress);
        AddField(2, NearAddress);
        AddGame(1);
        AddGame(2);

        var result = _service.GetRecommended(User, UserLatitude, UserLongitude);

        Assert.Equal(new[] { 2, 1 }, result.Select(r => r.Game.FootballFieldId));
    }

    [Fact]
    public void GetRecommended_WithLocation_ReturnsDistanceInKilometers()
    {
        AddField(1, NearAddress);
        AddGame(1);

        var result = _service.GetRecommended(User, UserLatitude, UserLongitude);

        Assert.Equal(1.11, Assert.Single(result).DistanceKm!.Value, 2);
    }

    [Fact]
    public void GetRecommended_WithoutLocation_DistanceIsNull()
    {
        AddField(1);
        AddGame(1);

        var result = _service.GetRecommended(User);

        Assert.Null(Assert.Single(result).DistanceKm);
    }

    [Fact]
    public void GetRecommended_FieldWithoutCoordinates_DistanceIsNullButGameIsRecommended()
    {
        AddField(1, "Gedimino pr. 9");
        AddGame(1);

        var result = _service.GetRecommended(User, UserLatitude, UserLongitude);

        Assert.Null(Assert.Single(result).DistanceKm);
    }

    // ---------- other criteria ----------

    [Fact]
    public void GetRecommended_SoonerGameComesFirst()
    {
        AddField(1);
        AddGame(1, hoursFromNow: 48);
        var sooner = AddGame(1, hoursFromNow: 12);

        var result = _service.GetRecommended(User);

        Assert.Equal(sooner.Id, result[0].Game.Id);
    }

    [Fact]
    public void GetRecommended_GameWithMoreFreeSpotsComesFirst()
    {
        AddField(1);
        AddGame(1, players: new[] { "a", "b", "c", "d", "e", "f" });
        var emptier = AddGame(1);

        var result = _service.GetRecommended(User);

        Assert.Equal(emptier.Id, result[0].Game.Id);
    }

    [Fact]
    public void GetRecommended_GameOnMorePopularFieldComesFirst()
    {
        AddField(1);
        AddField(2);
        AddGame(1, hoursFromNow: -48);
        AddGame(1, hoursFromNow: -24);
        AddGame(2);
        AddGame(1);

        var result = _service.GetRecommended(User);

        Assert.Equal(new[] { 1, 2 }, result.Select(r => r.Game.FootballFieldId));
    }

    [Fact]
    public void GetRecommended_SameSportAsUsersEarlierGamesComesFirst()
    {
        AddField(1, gameType: "Football");
        AddField(2, gameType: "Basketball");
        AddField(3, gameType: "Basketball");
        AddGame(3, hoursFromNow: -24, players: new[] { User });
        AddGame(1);
        AddGame(2);

        var result = _service.GetRecommended(User);

        Assert.Equal(new[] { 2, 1 }, result.Select(r => r.Game.FootballFieldId));
    }

    [Fact]
    public void GetRecommended_Scores_AreBetweenZeroAndOne()
    {
        AddField(1, NearAddress);
        AddField(2, FarAddress, "Basketball");
        AddGame(1, hoursFromNow: 1);
        AddGame(2, hoursFromNow: 500, players: new[] { "a", "b", "c" });

        var result = _service.GetRecommended(User, UserLatitude, UserLongitude);

        Assert.All(result, r => Assert.InRange(r.Score, 0, 1));
    }
}
