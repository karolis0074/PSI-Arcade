using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public class GameManagerService
{
    private readonly GameRepository _gameRepo;
    private readonly SportFieldRepository _sportFieldRepo;

    public GameManagerService(GameRepository gameRepo, SportFieldRepository sportFieldRepo)
    {
        _gameRepo = gameRepo;
        _sportFieldRepo = sportFieldRepo;
    }

    public IReadOnlyList<Game> GetAll() => _gameRepo.GetAll();

    public Game? GetById(int id) => _gameRepo.GetById(id);

    public CreateGameResult Create(string username, CreateGameRequest request)
    {
        if (request.StartTime <= DateTime.UtcNow)
            return new CreateGameResult(null, "StartTime must be in the future");

        var field = _sportFieldRepo.GetById(request.FootballFieldId);
        if (field is null)
            return new CreateGameResult(null, "Sport field not found");

        if (request.MaxPlayers > field.Capacity)
            return new CreateGameResult(null, $"MaxPlayers cannot exceed the field capacity ({field.Capacity})");

        var game = new Game
        {
            FootballFieldId = request.FootballFieldId,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers,
            CreatedByUsername = username
        };
        game.JoinedUsernames.Add(username);
        game.UpdateStatus();

        return new CreateGameResult(_gameRepo.Add(game), null);
    }

    public DeleteGameResult Delete(int id, string username)
    {
        var game = _gameRepo.GetById(id);

        if (game is null)
            return DeleteGameResult.NotFound;

        if (!IsHost(username, game))
            return DeleteGameResult.NotHost;

        if (!_gameRepo.Delete(id))
            return DeleteGameResult.NotFound;

        return DeleteGameResult.Success;
    }

    public string? GetHostUsername(Game game) => game?.CreatedByUsername;

    public string? GetHostUsername(int id) => GetHostUsername(_gameRepo.GetById(id));

    public bool IsHost(string username, Game game)
    {
        if (username is null || game is null)
            return false;

        string? hostUsername = GetHostUsername(game);
        if (hostUsername is null)
            return false;

        return hostUsername.Equals(username);
    }

    public bool IsHost(string username, int gameId) => IsHost(username, _gameRepo.GetById(gameId));
}
