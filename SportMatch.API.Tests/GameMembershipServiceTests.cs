using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests;

public class GameMembershipServiceTests
{
    // adds a game that starts tomorrow, so it is not started yet
    private static async Task<Game> AddGameAsync(AppDbContext dbContext, int maxPlayers)
    {
        var game = new Game
        {
            FootballFieldId = 1,
            CreatedByUsername = "host",
            StartTime = DateTime.UtcNow.AddDays(1),
            MaxPlayers = maxPlayers
        };

        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        return game;
    }

    [Fact]
    public async Task Join_GameHasFreeSpot_ReturnsSuccess()
    {
        // arrange
        var dbContext = await TestDatabase.CreateAsync();
        var game = await AddGameAsync(dbContext, maxPlayers: 2);
        var service = new GameMembershipService(new GameRepository(dbContext), new SystemClock());

        // act
        var result = await service.JoinAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.Success, result);
        Assert.Contains("jonas", (await service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Join_GameIsFull_ReturnsGameFull()
    {
        // arrange
        var dbContext = await TestDatabase.CreateAsync();
        var game = await AddGameAsync(dbContext, maxPlayers: 1);
        var service = new GameMembershipService(new GameRepository(dbContext), new SystemClock());
        await service.JoinAsync(game.Id, "petras");    // first player takes the only spot

        // act
        var result = await service.JoinAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.GameFull, result);
        Assert.DoesNotContain("jonas", (await service.GetGameAsync(game.Id))!.JoinedUsernames);
    }

    [Fact]
    public async Task Leave_UserNotJoined_ReturnsNotJoined()
    {
        // arrange
        var dbContext = await TestDatabase.CreateAsync();
        var game = await AddGameAsync(dbContext, maxPlayers: 2);
        var service = new GameMembershipService(new GameRepository(dbContext), new SystemClock());

        // act
        var result = await service.LeaveAsync(game.Id, "jonas");

        // assert
        Assert.Equal(MembershipResult.NotJoined, result);
    }
}
