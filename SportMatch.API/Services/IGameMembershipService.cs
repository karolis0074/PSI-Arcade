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
    string GetHostUsername(Game game);
    bool IsHost(string username, Game game);
    Task<string?> GetHostUsernameAsync(int id);
    Task<bool> IsHostAsync(string username, int gameId);
}