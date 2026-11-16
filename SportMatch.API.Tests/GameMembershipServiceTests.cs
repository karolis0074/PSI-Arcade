using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests;

public class GameMembershipServiceTests
{
    // adds a game that starts tomorrow, so it is not started yet
    private static Game AddGame(AppDbContext dbContext, int maxPlayers)
    {
        var game = new Game
        {
            FootballFieldId = 1,
            CreatedByUsername = "host",
            StartTime = DateTime.UtcNow.AddDays(1),
            MaxPlayers = maxPlayers
        };

        dbContext.Games.Add(game);
        dbContext.SaveChanges();

        return game;
    }

    [Fact]
    public void Join_GameHasFreeSpot_ReturnsSuccess()
    {
        // arrange
        var dbContext = TestDatabase.Create();
        var game = AddGame(dbContext, maxPlayers: 2);
        var service = new GameMembershipService(new GameRepository(dbContext));

        // act
        var result = service.Join(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        Assert.Contains("jonas", service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Join_GameIsFull_ReturnsGameFull()
    {
        // arrange
        var dbContext = TestDatabase.Create();
        var game = AddGame(dbContext, maxPlayers: 1);
        var service = new GameMembershipService(new GameRepository(dbContext));
        service.Join(game.Id, "petras");    // first player takes the only spot

        // act
        var result = service.Join(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameFull, result);
        Assert.DoesNotContain("jonas", service.GetGame(game.Id)!.JoinedUsernames);
    }

    [Fact]
    public void Leave_UserNotJoined_ReturnsNotJoined()
    {
        // arrange
        var dbContext = TestDatabase.Create();
        var game = AddGame(dbContext, maxPlayers: 2);
        var service = new GameMembershipService(new GameRepository(dbContext));

        // act
        var result = service.Leave(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.NotJoined, result);
    }
}
