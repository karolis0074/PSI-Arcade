using SportMatch.API.Data;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Services;

// Unit tests: GameManagerService gets FAKE repositories (lists in memory),
// and every result is checked only through the service's public methods.
public class GameManagerServiceTests
{
    private const int FieldId = 1;
    private const int FieldCapacity = 10;

    private readonly FakeGameRepository _gameRepository = new();
    private readonly FakeSportFieldRepository _fieldRepository = new();
    private readonly FakeClock _clock = new();
    private readonly GameManagerService _service;

    // xUnit creates a new instance for every test, so every test starts with clean fakes
    public GameManagerServiceTests()
    {
        _fieldRepository.Add(new SportField { Id = FieldId, Name = "Test field", Capacity = FieldCapacity });
        _service = new GameManagerService(_gameRepository, _fieldRepository, _clock);
    }

    private CreateGameRequest ValidRequest(int maxPlayers = 4) =>
        new(FieldId, _clock.UtcNow.AddDays(1), maxPlayers);

    // puts a game straight into the fake repository
    private Game AddGame(string host)
    {
        return _gameRepository.Add(new Game
        {
            FootballFieldId = FieldId,
            CreatedByUsername = host,
            StartTime = _clock.UtcNow.AddDays(1),
            MaxPlayers = 4
        });
    }

    // ---------- Create ----------

    [Fact]
    public void Create_ValidRequest_SavesGameWithHostJoined()
    {
        var request = ValidRequest(maxPlayers: 4);

        var result = _service.Create("host", request);

        Assert.Null(result.Error);
        var saved = _service.GetById(result.Game!.Id);
        Assert.NotNull(saved);
        Assert.Equal("host", saved!.CreatedByUsername);
        Assert.Equal(FieldId, saved.FootballFieldId);
        Assert.Equal(request.StartTime, saved.StartTime);
        Assert.Equal(4, saved.MaxPlayers);
        Assert.Equal("host", Assert.Single(saved.JoinedUsernames));
        Assert.Equal(GameStatus.Open, saved.Status);
    }

    [Fact]
    public void Create_StartTimeInPast_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(FieldId, _clock.UtcNow.AddHours(-1), 4);

        var result = _service.Create("host", request);

        Assert.Null(result.Game);
        Assert.Equal("StartTime must be in the future", result.Error);
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Create_StartTimeIsNow_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(FieldId, _clock.UtcNow, 4);

        var result = _service.Create("host", request);

        Assert.Null(result.Game);
        Assert.Equal("StartTime must be in the future", result.Error);
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Create_FieldDoesNotExist_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(999, _clock.UtcNow.AddDays(1), 4);

        var result = _service.Create("host", request);

        Assert.Null(result.Game);
        Assert.Equal("Sport field not found", result.Error);
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Create_MaxPlayersOverFieldCapacity_ReturnsErrorAndDoesNotSave()
    {
        var request = ValidRequest(maxPlayers: FieldCapacity + 1);

        var result = _service.Create("host", request);

        Assert.Null(result.Game);
        Assert.Equal($"MaxPlayers cannot exceed the field capacity ({FieldCapacity})", result.Error);
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Create_MaxPlayersEqualsFieldCapacity_SavesGame()
    {
        var request = ValidRequest(maxPlayers: FieldCapacity);

        var result = _service.Create("host", request);

        Assert.Null(result.Error);
        Assert.NotNull(_service.GetById(result.Game!.Id));
    }

    // ---------- Delete ----------

    [Fact]
    public void Delete_UserIsHost_ReturnsSuccessAndRemovesGame()
    {
        var game = AddGame(host: "host");

        var result = _service.Delete(game.Id, "host");

        Assert.Equal(DeleteGameResult.Success, result);
        Assert.Null(_service.GetById(game.Id));
    }

    [Fact]
    public void Delete_UserIsNotHost_ReturnsNotHostAndKeepsGame()
    {
        var game = AddGame(host: "host");

        var result = _service.Delete(game.Id, "jonas");

        Assert.Equal(DeleteGameResult.NotHost, result);
        Assert.NotNull(_service.GetById(game.Id));
    }

    [Fact]
    public void Delete_GameDoesNotExist_ReturnsNotFound()
    {
        const int missingId = 999;

        var result = _service.Delete(missingId, "host");

        Assert.Equal(DeleteGameResult.NotFound, result);
    }

    [Fact]
    public void Delete_RepositoryDeleteFails_ReturnsNotFound()
    {
        // the game exists, but another request deletes it first,
        // so deleting it in the database fails (only possible to set up with a fake)
        var game = AddGame(host: "host");
        _gameRepository.DeleteShouldFail = true;

        var result = _service.Delete(game.Id, "host");

        Assert.Equal(DeleteGameResult.NotFound, result);
    }

    // ---------- GetHostUsername ----------

    [Fact]
    public void GetHostUsername_GameExists_ReturnsCreator()
    {
        var game = AddGame(host: "host");

        var host = _service.GetHostUsername(game.Id);

        Assert.Equal("host", host);
    }

    [Fact]
    public void GetHostUsername_GameObjectGiven_ReturnsCreator()
    {
        var game = AddGame(host: "host");

        var host = _service.GetHostUsername(game);

        Assert.Equal("host", host);
    }

    [Fact]
    public void GetHostUsername_GameDoesNotExist_ReturnsNull()
    {
        const int missingId = 999;

        var host = _service.GetHostUsername(missingId);

        Assert.Null(host);
    }

    // ---------- IsHost ----------

    [Theory]
    [InlineData("host", true)]
    [InlineData("jonas", false)]
    public void IsHost_GameIdGiven_ReturnsTrueOnlyForCreator(string username, bool expected)
    {
        var game = AddGame(host: "host");

        var isHost = _service.IsHost(username, game.Id);

        Assert.Equal(expected, isHost);
    }

    [Theory]
    [InlineData("host", true)]
    [InlineData("jonas", false)]
    public void IsHost_GameObjectGiven_ReturnsTrueOnlyForCreator(string username, bool expected)
    {
        var game = AddGame(host: "host");

        var isHost = _service.IsHost(username, game);

        Assert.Equal(expected, isHost);
    }

    [Fact]
    public void IsHost_UsernameIsNull_ReturnsFalse()
    {
        var game = AddGame(host: "host");

        var isHost = _service.IsHost(null!, game.Id);

        Assert.False(isHost);
    }

    [Fact]
    public void IsHost_GameDoesNotExist_ReturnsFalse()
    {
        const int missingId = 999;

        var isHost = _service.IsHost("host", missingId);

        Assert.False(isHost);
    }
}
