using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Services;

// unit tests: GameMembershipService gets a fake repository and a fake clock, so no database or real time is used
public class GameMembershipServiceTests
{
    private const int MissingGameId = 999;

    private readonly FakeGameRepository _gameRepository = new();
    private readonly FakeClock _clock = new();
    private readonly GameMembershipService _service;

    public GameMembershipServiceTests()
    {
        _service = new GameMembershipService(_gameRepository, _clock);
    }

    // adds a game that starts tomorrow, the host is already joined like in a created game
    private Game AddGame(int maxPlayers)
    {
        var game = new Game
        {
            FootballFieldId = 1,
            CreatedByUsername = "host",
            StartTime = _clock.UtcNow.AddDays(1),
            MaxPlayers = maxPlayers
        };
        game.JoinedUsernames.Add("host");
        game.UpdateStatus();

        return _gameRepository.Add(game);
    }

    // ---------- Join ----------

    [Fact]
    public async Task Join_GameDoesNotExist_ReturnsGameNotFound()
    {
        // act
        var result = await _service.JoinAsync(MissingGameId, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameNotFound, result);
    }

    [Fact]
    public async Task Join_GameAlreadyStarted_ReturnsGameStarted()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);
        game.StartTime = _clock.UtcNow.AddHours(-1);

        // act
        var result = await _service.JoinAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.DoesNotContain("jonas", (await _service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Join_UserAlreadyJoined_ReturnsAlreadyJoined()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = await _service.JoinAsync(game.Id, "host");

        // assert
        Assert.Equal(MembershipResult.AlreadyJoined, result);
        Assert.Single((await _service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Join_GameIsFull_ReturnsGameFull()
    {
        // arrange
        var game = AddGame(maxPlayers: 1);    // host takes the only spot

        // act
        var result = await _service.JoinAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameFull, result);
        Assert.DoesNotContain("jonas", (await _service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Join_LastFreeSpot_AddsUserAndGameBecomesFull()
    {
        // arrange
        var game = AddGame(maxPlayers: 2);

        // act
        var result = await _service.JoinAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        var savedGame = (await _service.GetGameAsync(game.Id))!;
        Assert.Contains("jonas", savedGame.JoinedUsernames);
        Assert.Equal(GameStatus.Full, savedGame.Status);
    }

    // ---------- Leave ----------

    [Fact]
    public async Task Leave_GameDoesNotExist_ReturnsGameNotFound()
    {
        // act
        var result = await _service.LeaveAsync(MissingGameId, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameNotFound, result);
    }

    [Fact]
    public async Task Leave_GameAlreadyStarted_ReturnsGameStarted()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);
        await _service.JoinAsync(game.Id, "jonas");
        game.StartTime = _clock.UtcNow.AddHours(-1);

        // act
        var result = await _service.LeaveAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.Contains("jonas", (await _service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Leave_UserNotJoined_ReturnsNotJoined()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = await _service.LeaveAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.NotJoined, result);
    }

    [Fact]
    public async Task Leave_UserIsHost_ReturnsHostCannotLeave()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = await _service.LeaveAsync(game.Id, "host");

        // assert
        Assert.Equal(MembershipResult.HostCannotLeave, result);
        Assert.Contains("host", (await _service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Leave_JoinedUser_RemovesUserAndGameIsOpenAgain()
    {
        // arrange
        var game = AddGame(maxPlayers: 2);
        await _service.JoinAsync(game.Id, "jonas");    // now the game is full

        // act
        var result = await _service.LeaveAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        var savedGame = (await _service.GetGameAsync(game.Id))!;
        Assert.DoesNotContain("jonas", savedGame.JoinedUsernames);
        Assert.Equal(GameStatus.Open, savedGame.Status);
    }

    // ---------- GetSpots ----------

    [Fact]
    public async Task GetSpots_GameExists_ReturnsJoinedMaxAndFree()
    {
        // arrange
        var game = AddGame(maxPlayers: 10);

        // act
        var spots = await _service.GetSpotsAsync(game.Id);

        // assert
        Assert.NotNull(spots);
        Assert.Equal(1, spots.Value.Joined);
        Assert.Equal(10, spots.Value.Max);
        Assert.Equal(9, spots.Value.Free);
    }

    [Fact]
    public async Task GetSpots_GameDoesNotExist_ReturnsNull()
    {
        // act
        var spots = await _service.GetSpotsAsync(MissingGameId);

        // assert
        Assert.Null(spots);
    }
}