using SportMatch.API.Models;
using SportMatch.API.Results;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// results are set by the test
public class FakeAccountService : IAccountService
{
    public RegisterResult RegisterResult { get; set; } = RegisterResult.Success;
    public AccountResult ChangePasswordResult { get; set; } = AccountResult.Success;
    public AccountResult DeleteResult { get; set; } = AccountResult.Success;

    public string? LastUsername { get; private set; }

    public Task<RegisterResult> RegisterAsync(string username, string password)
    {
        LastUsername = username;
        return Task.FromResult(RegisterResult);
    }

    public Task<AccountResult> ChangePasswordAsync(Account? account, string newPassword)
    {
        LastUsername = account?.Username;
        return Task.FromResult(ChangePasswordResult);
    }

    public Task<AccountResult> ChangePasswordAsync(string username, string newPassword)
    {
        LastUsername = username;
        return Task.FromResult(ChangePasswordResult);
    }

    public Task<AccountResult> DeleteAccountAsync(string username)
    {
        LastUsername = username;
        return Task.FromResult(DeleteResult);
    }
}
