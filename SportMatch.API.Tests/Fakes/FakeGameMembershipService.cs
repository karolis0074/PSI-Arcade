using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake service for controller tests: it has no join/leave rules,
// it just returns what the test sets and remembers what the controller sent
public class FakeGameMembershipService : IGameMembershipService
{
    // what Join and Leave return
    public MembershipResult Result { get; set; } = MembershipResult.Success;

    // what GetGame and GetSpots return
    public Game? Game { get; set; }
    public GameSpots? Spots { get; set; }

    // what the controller passed to Join or Leave
    public int LastGameId { get; private set; }
    public string? LastUsername { get; private set; }

    // returns the game only when the controller asked for the right id
    public Game? GetGame(int gameId)
    {
        if (Game is not null && Game.Id == gameId)
            return Game;

        return null;
    }

    public int GetFreeSpots(Game game) => game.MaxPlayers - game.JoinedUsernames.Count;

    public GameSpots? GetSpots(int gameId)
    {
        LastGameId = gameId;
        return Spots;
    }

    public MembershipResult Join(int gameId, string username)
    {
        LastGameId = gameId;
        LastUsername = username;
        return Result;
    }

    public MembershipResult Leave(int gameId, string username)
    {
        LastGameId = gameId;
        LastUsername = username;
        return Result;
    }
}
