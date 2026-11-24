using SportMatch.API.Models;

namespace SportMatch.API.Services;

public interface IAuthService
{
    // no database access here (the account is already loaded), so it stays synchronous
    bool VerifyPassword(Account? account, string password);
    Task<bool> CheckLoginAsync(string username, string password);
    Task<string?> LoginAsync(string username, string password);
}
