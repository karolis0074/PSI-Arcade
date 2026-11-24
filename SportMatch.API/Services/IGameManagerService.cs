using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public interface IGameManagerService
{
    Task<IReadOnlyList<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<CreateGameResult> CreateAsync(string username, CreateGameRequest request);
    Task<DeleteGameResult> DeleteAsync(int id, string username);

    // these two only read an already loaded Game, so they stay synchronous
    string GetHostUsername(Game game);
    bool IsHost(string username, Game game);

    // these two load the game from the database first
    Task<string?> GetHostUsernameAsync(int id);
    Task<bool> IsHostAsync(string username, int gameId);
}
