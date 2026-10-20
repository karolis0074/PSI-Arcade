using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services;

public class AccountService
{
    private readonly AccountRepository _accountRepo;
    private readonly CryptService _cryptService;

    public AccountService(AccountRepository accountRepo, CryptService cryptService)
    {
        _accountRepo = accountRepo;
        _cryptService = cryptService;
    }

    public string? GetDisplayName(string username)
    {
        var account = _accountRepo.GetByUsername(username);
        return account?.DisplayName;
    }

    public RegisterResult Register(string displayName, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(displayName) ||
            string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
            return RegisterResult.InvalidInput;

        var passwordHash = _cryptService.Hash(password);
        var account = new Account(displayName, username, passwordHash);
        return _accountRepo.Add(account);
    }

    public AccountResult ChangePassword(Account? account, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return AccountResult.InvalidInput;
        if (account is null)
            return AccountResult.NotFound;

        account.PasswordHash = _cryptService.Hash(newPassword);
        _accountRepo.Update(account);
        return AccountResult.Success;
    }

    public AccountResult ChangePassword(string username, string newPassword)
    {
        var account = _accountRepo.GetByUsername(username);
        return ChangePassword(account, newPassword);
    }

    public AccountResult ChangeDisplayName(Account? account, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            return AccountResult.InvalidInput;
        if (account is null)
            return AccountResult.NotFound;

        account.DisplayName = newName;
        _accountRepo.Update(account);
        return AccountResult.Success;
    }

    public AccountResult ChangeDisplayName(string username, string newName)
    {
        var account = _accountRepo.GetByUsername(username);
        return ChangeDisplayName(account, newName);
    }

    public AccountResult DeleteAccount(string username) => _accountRepo.Delete(username);
}
