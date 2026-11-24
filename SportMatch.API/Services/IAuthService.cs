using SportMatch.API.Models;
namespace SportMatch.API.Services;
public interface IAuthService
{
    bool CheckLogin(Account? account, string password);
    Task<bool> CheckLoginAsync(string username, string password);
    Task<string?> LoginAsync(string username, string password);
}