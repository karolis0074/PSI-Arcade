using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake service for controller tests: keeps games in a list, and the results of
// Create and Delete are set by the test, so no business logic is copied here
public class FakeGameManagerService : IGameManagerService
{
    private readonly List<Game> _games = new();
    private int _nextId = 1;

    // when set, Create fails with this error message
    public string? CreateError { get; set; }

    // the result Delete returns
    public DeleteGameResult DeleteResult { get; set; } = DeleteGameResult.Success;

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

    public DeleteGameResult Delete(int id, string username) => DeleteResult;

    public string? GetHostUsername(Game game) => game.CreatedByUsername;
    public string? GetHostUsername(int id) => GetById(id)?.CreatedByUsername;
    public bool IsHost(string username, Game game) => game.CreatedByUsername == username;
    public bool IsHost(string username, int gameId) => GetHostUsername(gameId) == username;
}
