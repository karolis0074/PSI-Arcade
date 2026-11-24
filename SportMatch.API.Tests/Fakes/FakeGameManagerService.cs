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

    public Task<IReadOnlyList<Game>> GetAllAsync() =>
        Task.FromResult<IReadOnlyList<Game>>(_games);

    public Task<Game?> GetByIdAsync(int id) =>
        Task.FromResult(_games.FirstOrDefault(g => g.Id == id));

    public Task<CreateGameResult> CreateAsync(string username, CreateGameRequest request)
    {
        if (CreateError is not null)
            return Task.FromResult(new CreateGameResult(null, CreateError));

        var game = new Game
        {
            Id = _nextId++,
            CreatedByUsername = username,
            FootballFieldId = request.FootballFieldId,
            StartTime = request.StartTime,
            MaxPlayers = request.MaxPlayers
        };
        _games.Add(game);
        return Task.FromResult(new CreateGameResult(game, null));
    }

    public Task<DeleteGameResult> DeleteAsync(int id, string username) => Task.FromResult(DeleteResult);

    public string GetHostUsername(Game game) => game.CreatedByUsername;
    public bool IsHost(string username, Game game) => game.CreatedByUsername == username;

    public async Task<string?> GetHostUsernameAsync(int id) => (await GetByIdAsync(id))?.CreatedByUsername;
    public async Task<bool> IsHostAsync(string username, int gameId) => await GetHostUsernameAsync(gameId) == username;
}
