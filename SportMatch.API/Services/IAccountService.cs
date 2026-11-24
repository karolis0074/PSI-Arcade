using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public interface IAccountService
{
    Task<RegisterResult> RegisterAsync(string username, string password);
    Task<AccountResult> ChangePasswordAsync(Account? account, string newPassword);
    Task<AccountResult> ChangePasswordAsync(string username, string newPassword);
    Task<AccountResult> DeleteAccountAsync(string username);
}
