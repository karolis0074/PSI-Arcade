using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;
using SportMatch.API.Services;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Integration.Services;

// Integration tests: GameMembershipService together with the real GameRepository
// on an in-memory SQLite database (a new one for every test). Only the clock is faked.
public class GameMembershipServiceTests
{
    private readonly AppDbContext _dbContext = TestDatabase.Create();
    private readonly FakeClock _clock = new();
    private readonly GameMembershipService _service;

    public GameMembershipServiceTests()
    {
        _service = new GameMembershipService(new GameRepository(_dbContext), _clock);
    }

    // adds a game that starts tomorrow according to the fake clock, so it is not started yet
    private Game AddGame(int maxPlayers)
    {
        var game = new Game
        {
            FootballFieldId = 1,
            CreatedByUsername = "host",
            StartTime = _clock.UtcNow.AddDays(1),
            MaxPlayers = maxPlayers
        };

        _dbContext.Games.Add(game);
        _dbContext.SaveChanges();

        return game;
    }

    [Fact]
    public void Join_GameHasFreeSpot_ReturnsSuccess()
    {
        var game = AddGame(maxPlayers: 2);

        var result = _service.Join(game.Id, "jonas");

        Assert.Equal(MembershipResult.Success, result);
        Assert.Contains("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_GameIsFull_ReturnsGameFull()
    {
        var game = AddGame(maxPlayers: 1);
        _service.Join(game.Id, "petras");    // first player takes the only spot

        var result = _service.Join(game.Id, "jonas");

        Assert.Equal(MembershipResult.GameFull, result);
        Assert.DoesNotContain("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_GameAlreadyStarted_ReturnsGameStarted()
    {
        var game = AddGame(maxPlayers: 2);
        _clock.UtcNow = game.StartTime;      // time moves to the moment the game starts

        var result = _service.Join(game.Id, "jonas");

        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.DoesNotContain("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Leave_UserNotJoined_ReturnsNotJoined()
    {
        var game = AddGame(maxPlayers: 2);

        var result = _service.Leave(game.Id, "jonas");

        Assert.Equal(MembershipResult.NotJoined, result);
    }

    [Fact]
    public void Leave_GameAlreadyStarted_ReturnsGameStarted()
    {
        var game = AddGame(maxPlayers: 2);
        _service.Join(game.Id, "jonas");
        _clock.UtcNow = game.StartTime.AddMinutes(1);

        var result = _service.Leave(game.Id, "jonas");

        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.Contains("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }
}
