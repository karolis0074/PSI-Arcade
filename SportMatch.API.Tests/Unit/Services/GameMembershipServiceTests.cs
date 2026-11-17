using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;
using SportMatch.API.Tests.Fakes;

namespace SportMatch.API.Tests.Unit.Services;

// unit tests: GameMembershipService gets a fake repository, so no database is used
public class GameMembershipServiceTests
{
    private const int MissingGameId = 999;

    private readonly FakeGameRepository _gameRepository = new();
    private readonly GameMembershipService _service;

    public GameMembershipServiceTests()
    {
        _service = new GameMembershipService(_gameRepository);
    }

    // adds a game that starts tomorrow, the host is already joined like in a created game
    private Game AddGame(int maxPlayers)
    {
        var game = new Game
        {
            FootballFieldId = 1,
            CreatedByUsername = "host",
            StartTime = DateTime.UtcNow.AddDays(1),
            MaxPlayers = maxPlayers
        };
        game.JoinedUsernames.Add("host");
        game.UpdateStatus();

        return _gameRepository.Add(game);
    }

    // ---------- Join ----------

    [Fact]
    public void Join_GameDoesNotExist_ReturnsGameNotFound()
    {
        // act
        var result = _service.Join(MissingGameId, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameNotFound, result);
    }

    [Fact]
    public void Join_GameAlreadyStarted_ReturnsGameStarted()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);
        game.StartTime = DateTime.UtcNow.AddHours(-1);

        // act
        var result = _service.Join(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.DoesNotContain("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_UserAlreadyJoined_ReturnsAlreadyJoined()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = _service.Join(game.Id, "host");

        // assert
        Assert.Equal(MembershipResult.AlreadyJoined, result);
        Assert.Single(_service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_GameIsFull_ReturnsGameFull()
    {
        // arrange
        var game = AddGame(maxPlayers: 1);    // host takes the only spot

        // act
        var result = _service.Join(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameFull, result);
        Assert.DoesNotContain("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_LastFreeSpot_AddsUserAndGameBecomesFull()
    {
        // arrange
        var game = AddGame(maxPlayers: 2);

        // act
        var result = _service.Join(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        var savedGame = _service.GetGame(game.Id)!;
        Assert.Contains("jonas", savedGame.JoinedUsernames);
        Assert.Equal(GameStatus.Full, savedGame.Status);
    }

    // ---------- Leave ----------

    [Fact]
    public void Leave_GameDoesNotExist_ReturnsGameNotFound()
    {
        // act
        var result = _service.Leave(MissingGameId, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameNotFound, result);
    }

    [Fact]
    public void Leave_GameAlreadyStarted_ReturnsGameStarted()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);
        _service.Join(game.Id, "jonas");
        game.StartTime = DateTime.UtcNow.AddHours(-1);

        // act
        var result = _service.Leave(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameStarted, result);
        Assert.Contains("jonas", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Leave_UserNotJoined_ReturnsNotJoined()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = _service.Leave(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.NotJoined, result);
    }

    [Fact]
    public void Leave_UserIsHost_ReturnsHostCannotLeave()
    {
        // arrange
        var game = AddGame(maxPlayers: 4);

        // act
        var result = _service.Leave(game.Id, "host");

        // assert
        Assert.Equal(MembershipResult.HostCannotLeave, result);
        Assert.Contains("host", _service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Leave_JoinedUser_RemovesUserAndGameIsOpenAgain()
    {
        // arrange
        var game = AddGame(maxPlayers: 2);
        _service.Join(game.Id, "jonas");    // now the game is full

        // act
        var result = _service.Leave(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        var savedGame = _service.GetGame(game.Id)!;
        Assert.DoesNotContain("jonas", savedGame.JoinedUsernames);
        Assert.Equal(GameStatus.Open, savedGame.Status);
    }

    // ---------- GetSpots ----------

    [Fact]
    public void GetSpots_GameExists_ReturnsJoinedMaxAndFree()
    {
        // arrange
        var game = AddGame(maxPlayers: 10);

        // act
        var spots = _service.GetSpots(game.Id);

        // assert
        Assert.NotNull(spots);
        Assert.Equal(1, spots.Value.Joined);
        Assert.Equal(10, spots.Value.Max);
        Assert.Equal(9, spots.Value.Free);
    }

    [Fact]
    public void GetSpots_GameDoesNotExist_ReturnsNull()
    {
        // act
        var spots = _service.GetSpots(MissingGameId);

        // assert
        Assert.Null(spots);
    }
}
