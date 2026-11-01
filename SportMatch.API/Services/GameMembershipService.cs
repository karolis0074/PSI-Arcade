using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public class GameMembershipService
{
    // DI — GameRepository comes through the constructor
    private readonly GameRepository _gameRepository;

    public GameMembershipService(GameRepository gameRepository)
    {
        _gameRepository = gameRepository;
    }

    public Game? GetGame(int gameId) => _gameRepository.GetById(gameId);

    public int GetFreeSpots(Game game) => game.MaxPlayers - game.JoinedUsernames.Count;

    public GameSpots? GetSpots(int gameId)
    {
        var game = _gameRepository.GetById(gameId);

        if (game is null)
            return null;

        var spots = new GameSpots();
        spots.Joined = game.JoinedUsernames.Count;
        spots.Max = game.MaxPlayers;
        spots.Free = GetFreeSpots(game);

        return spots;
    }

    public MembershipResult Join(int gameId, string username)
    {
        var game = _gameRepository.GetById(gameId);

        if (game is null)
            return MembershipResult.GameNotFound;

        // can't join a game that already started
        if (game.StartTime <= DateTime.UtcNow)
            return MembershipResult.GameStarted;

        // user can't join the same game twice
        if (game.JoinedUsernames.Contains(username))
            return MembershipResult.AlreadyJoined;

        // no free spots left
        if (GetFreeSpots(game) <= 0)
            return MembershipResult.GameFull;

        game.JoinedUsernames.Add(username);
        game.UpdateStatus();

        _gameRepository.Update(game);

        return MembershipResult.Success;
    }

    public MembershipResult Leave(int gameId, string username)
    {
        var game = _gameRepository.GetById(gameId);

        if (game is null)
            return MembershipResult.GameNotFound;

        // can't leave a game that already started
        if (game.StartTime <= DateTime.UtcNow)
            return MembershipResult.GameStarted;

        // user has to be in the game to leave it
        if (!game.JoinedUsernames.Contains(username))
            return MembershipResult.NotJoined;

        game.JoinedUsernames.Remove(username);
        game.UpdateStatus();

        _gameRepository.Update(game);

        return MembershipResult.Success;
    }
}
