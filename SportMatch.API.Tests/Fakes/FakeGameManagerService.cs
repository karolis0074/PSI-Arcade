using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake service for controller tests: keeps games in a list and has only the
// simplest logic, so the controller's responses depend on who is logged in
public class FakeGameManagerService : IGameManagerService
{
    private readonly List<Game> _games = new();
    private int _nextId = 1;

    // when set, Create fails with this error message
    public string? CreateError { get; set; }

    public Game AddGame(string host)
    {
        var game = new Game { Id = _nextId++, CreatedByUsername = host };
        _games.Add(game);
        return game;
    }

    public IReadOnlyList<Game> GetAll() => _games;

    public Game? GetById(int id) => _games.FirstOrDefault(g => g.Id == id);

    public CreateGameResult Create(string username, CreateGameRequest request)
    {
        if (CreateError is not null)
            return new CreateGameResult(null, CreateError);

        var game = new Game
        {
            Id = _nextId++,
            CreatedByUsername = username,
            FootballFieldId = request.FootballFieldId,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers
        };
        _games.Add(game);
        return new CreateGameResult(game, null);
    }

    public DeleteGameResult Delete(int id, string username)
    {
        var game = GetById(id);
        if (game is null)
            return DeleteGameResult.NotFound;
        if (game.CreatedByUsername != username)
            return DeleteGameResult.NotHost;

        _games.Remove(game);
        return DeleteGameResult.Success;
    }

    public string? GetHostUsername(Game game) => game.CreatedByUsername;
    public string? GetHostUsername(int id) => GetById(id)?.CreatedByUsername;
    public bool IsHost(string username, Game game) => game.CreatedByUsername == username;
    public bool IsHost(string username, int gameId) => GetHostUsername(gameId) == username;
}
