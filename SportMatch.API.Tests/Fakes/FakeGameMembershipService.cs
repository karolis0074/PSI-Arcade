using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake service for controller tests: it has no join/leave rules,
// it just returns what the test sets and remembers what the controller sent
public class FakeGameMembershipService : IGameMembershipService
{
    // what JoinAsync and LeaveAsync return
    public MembershipResult Result { get; set; } = MembershipResult.Success;

    // what GetGameAsync and GetSpotsAsync return
    public Game? Game { get; set; }
    public GameSpots? Spots { get; set; }

    // what the controller passed to JoinAsync or LeaveAsync
    public int LastGameId { get; private set; }
    public string? LastUsername { get; private set; }

    // returns the game only when the controller asked for the right id
    public Task<Game?> GetGameAsync(int gameId)
    {
        if (Game is not null && Game.Id == gameId)
            return Task.FromResult<Game?>(Game);

        return Task.FromResult<Game?>(null);
    }

    public int GetFreeSpots(Game game) => game.MaxPlayers - game.JoinedUsernames.Count;

    public Task<GameSpots?> GetSpotsAsync(int gameId)
    {
        LastGameId = gameId;
        return Task.FromResult(Spots);
    }

    public Task<MembershipResult> JoinAsync(int gameId, string username)
    {
        LastGameId = gameId;
        LastUsername = username;
        return Task.FromResult(Result);
    }

    public Task<MembershipResult> LeaveAsync(int gameId, string username)
    {
        LastGameId = gameId;
        LastUsername = username;
        return Task.FromResult(Result);
    }
}