using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public class GameMembershipService : IGameMembershipService
{
    private readonly IGameRepository _gameRepository;
    private readonly IClock _clock;

    public GameMembershipService(IGameRepository gameRepository, IClock clock)
    {
        _gameRepository = gameRepository;
        _clock = clock;
    }

    public async Task<Game?> GetGameAsync(int gameId) =>
        await _gameRepository.GetByIdAsync(gameId);

    // pure calculation on an already loaded game, no await needed
    public int GetFreeSpots(Game game) => game.MaxPlayers - game.JoinedUsernames.Count;

    public async Task<GameSpots?> GetSpotsAsync(int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        if (game is null)
            return null;

        return new GameSpots
        {
            Joined = game.JoinedUsernames.Count,
            Max = game.MaxPlayers,
            Free = GetFreeSpots(game)
        };
    }

    public async Task<MembershipResult> JoinAsync(int gameId, string username)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);

        if (game is null)
            return MembershipResult.GameNotFound;
        if (game.StartTime <= _clock.UtcNow)
            return MembershipResult.GameStarted;
        if (game.JoinedUsernames.Contains(username, StringComparer.OrdinalIgnoreCase))
            return MembershipResult.AlreadyJoined;
        if (GetFreeSpots(game) <= 0)
            return MembershipResult.GameFull;

        game.JoinedUsernames.Add(username);
        game.UpdateStatus();
        await _gameRepository.UpdateAsync(game);

        return MembershipResult.Success;
    }

    public async Task<MembershipResult> LeaveAsync(int gameId, string username)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);

        if (game is null)
            return MembershipResult.GameNotFound;
        if (game.StartTime <= _clock.UtcNow)
            return MembershipResult.GameStarted;
        if (!game.JoinedUsernames.Contains(username, StringComparer.OrdinalIgnoreCase))
            return MembershipResult.NotJoined;
        if (game.CreatedByUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
            return MembershipResult.HostCannotLeave;

        game.JoinedUsernames.RemoveAll(u => u.Equals(username, StringComparison.OrdinalIgnoreCase));
        game.UpdateStatus();
        await _gameRepository.UpdateAsync(game);

        return MembershipResult.Success;
    }
}
