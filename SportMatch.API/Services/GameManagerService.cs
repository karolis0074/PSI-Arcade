using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

// creating and deleting games, with all the rules (start time, field exists, field capacity, host only)
public class GameManagerService : IGameManagerService
{
    private readonly IGameRepository _gameRepository;
    private readonly ISportFieldRepository _sportFieldRepository;
    private readonly IClock _clock;

    public GameManagerService(IGameRepository gameRepository, ISportFieldRepository sportFieldRepository, IClock clock)
    {
        _gameRepository = gameRepository;
        _sportFieldRepository = sportFieldRepository;
        _clock = clock;
    }

    public Task<IReadOnlyList<Game>> GetAllAsync() => _gameRepository.GetAllAsync();

    public Task<Game?> GetByIdAsync(int id) => _gameRepository.GetByIdAsync(id);

    public async Task<CreateGameResult> CreateAsync(string username, CreateGameRequest request)
    {
        if (request.StartTime <= _clock.UtcNow)
            return new CreateGameResult(null, "StartTime must be in the future");

        var field = await _sportFieldRepository.GetByIdAsync(request.FootballFieldId);
        if (field is null)
            return new CreateGameResult(null, "Sport field not found");

        if (request.MaxPlayers > field.Capacity)
            return new CreateGameResult(null, $"MaxPlayers cannot exceed the field capacity ({field.Capacity})");

        // host and status come from the server, never from the request body (no over-posting)
        var game = new Game
        {
            FootballFieldId = request.FootballFieldId,
            CreatedByUsername = username,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers,
            JoinedUsernames = new List<string> { username } // the host plays in their own game
        };
        game.UpdateStatus();

        await _gameRepository.AddAsync(game);
        return new CreateGameResult(game, null);
    }

    public async Task<DeleteGameResult> DeleteAsync(int id, string username)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        if (game is null)
            return DeleteGameResult.NotFound;

        if (!IsHost(username, game))
            return DeleteGameResult.NotHost;

        // the game can be deleted by another request in the meantime
        var deleted = await _gameRepository.DeleteAsync(game.Id);
        return deleted ? DeleteGameResult.Success : DeleteGameResult.NotFound;
    }

    public string GetHostUsername(Game game) => game.CreatedByUsername;

    public async Task<string?> GetHostUsernameAsync(int id)
    {
        var game = await _gameRepository.GetByIdAsync(id);
        return game is null ? null : GetHostUsername(game);
    }

    // usernames are case-insensitive everywhere in the app
    public bool IsHost(string username, Game game) =>
        game.CreatedByUsername.Equals(username, StringComparison.OrdinalIgnoreCase);

    public async Task<bool> IsHostAsync(string username, int gameId)
    {
        var game = await _gameRepository.GetByIdAsync(gameId);
        return game is not null && IsHost(username, game);
    }
}
