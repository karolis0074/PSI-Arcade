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
    public async Task Create_ValidRequest_SavesGameWithHostJoined()
    {
        var request = ValidRequest(maxPlayers: 4);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Error);
        var saved = await _service.GetByIdAsync(result.Game!.Id);
        Assert.NotNull(saved);
        Assert.Equal("host", saved!.CreatedByUsername);
        Assert.Equal(FieldId, saved.FootballFieldId);
        Assert.Equal(request.StartTime, saved.StartTime);
        Assert.Equal(4, saved.MaxPlayers);
        Assert.Equal("host", Assert.Single(saved.JoinedUsernames));
        Assert.Equal(GameStatus.Open, saved.Status);
    }

    [Fact]
    public async Task Create_StartTimeInPast_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(FieldId, _clock.UtcNow.AddHours(-1), 4);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Game);
        Assert.Equal("StartTime must be in the future", result.Error);
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Create_StartTimeIsNow_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(FieldId, _clock.UtcNow, 4);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Game);
        Assert.Equal("StartTime must be in the future", result.Error);
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Create_FieldDoesNotExist_ReturnsErrorAndDoesNotSave()
    {
        var request = new CreateGameRequest(999, _clock.UtcNow.AddDays(1), 4);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Game);
        Assert.Equal("Sport field not found", result.Error);
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Create_MaxPlayersOverFieldCapacity_ReturnsErrorAndDoesNotSave()
    {
        var request = ValidRequest(maxPlayers: FieldCapacity + 1);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Game);
        Assert.Equal($"MaxPlayers cannot exceed the field capacity ({FieldCapacity})", result.Error);
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Create_MaxPlayersEqualsFieldCapacity_SavesGame()
    {
        var request = ValidRequest(maxPlayers: FieldCapacity);

        var result = await _service.CreateAsync("host", request);

        Assert.Null(result.Error);
        Assert.NotNull(await _service.GetByIdAsync(result.Game!.Id));
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_UserIsHost_ReturnsSuccessAndRemovesGame()
    {
        var game = AddGame(host: "host");

        var result = await _service.DeleteAsync(game.Id, "host");

        Assert.Equal(DeleteGameResult.Success, result);
        Assert.Null(await _service.GetByIdAsync(game.Id));
    }

    [Fact]
    public async Task Delete_UserIsNotHost_ReturnsNotHostAndKeepsGame()
    {
        var game = AddGame(host: "host");

        var result = await _service.DeleteAsync(game.Id, "jonas");

        Assert.Equal(DeleteGameResult.NotHost, result);
        Assert.NotNull(await _service.GetByIdAsync(game.Id));
    }

    [Fact]
    public async Task Delete_GameDoesNotExist_ReturnsNotFound()
    {
        const int missingId = 999;

        var result = await _service.DeleteAsync(missingId, "host");

        Assert.Equal(DeleteGameResult.NotFound, result);
    }

    [Fact]
    public async Task Delete_RepositoryDeleteFails_ReturnsNotFound()
    {
        // the game exists, but another request deletes it first,
        // so deleting it in the database fails (only possible to set up with a fake)
        var game = AddGame(host: "host");
        _gameRepository.DeleteShouldFail = true;

        var result = await _service.DeleteAsync(game.Id, "host");

        Assert.Equal(DeleteGameResult.NotFound, result);
    }

    // ---------- GetHostUsername ----------

    [Fact]
    public async Task GetHostUsername_GameExists_ReturnsCreator()
    {
        var game = AddGame(host: "host");

        var host = await _service.GetHostUsernameAsync(game.Id);

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
    public async Task GetHostUsername_GameDoesNotExist_ReturnsNull()
    {
        const int missingId = 999;

        var host = await _service.GetHostUsernameAsync(missingId);

        Assert.Null(host);
    }

    // ---------- IsHost ----------

    [Theory]
    [InlineData("host", true)]
    [InlineData("jonas", false)]
    public async Task IsHost_GameIdGiven_ReturnsTrueOnlyForCreator(string username, bool expected)
    {
        var game = AddGame(host: "host");

        var isHost = await _service.IsHostAsync(username, game.Id);

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
    public async Task IsHost_UsernameIsNull_ReturnsFalse()
    {
        var game = AddGame(host: "host");

        var isHost = await _service.IsHostAsync(null!, game.Id);

        Assert.False(isHost);
    }

    [Fact]
    public async Task IsHost_GameDoesNotExist_ReturnsFalse()
    {
        const int missingId = 999;

        var isHost = await _service.IsHostAsync("host", missingId);

        Assert.False(isHost);
    }
}
