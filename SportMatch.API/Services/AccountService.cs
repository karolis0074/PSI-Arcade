using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;
using SportMatch.API.Exceptions;

namespace SportMatch.API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICryptService _cryptService;

    public AccountService(IAccountRepository accountRepository, ICryptService cryptService)
    {
        _accountRepository = accountRepository;
        _cryptService = cryptService;
    }

    public async Task<RegisterResult> RegisterAsync(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidAccountException("Username cannot be empty.");
        if (username.Length < 3 || username.Length > 20)
            throw new InvalidAccountException("Username must be 3-20 characters long.");
        if (!username.All(c => char.IsLetterOrDigit(c) || c == '_'))
            throw new InvalidAccountException("Username can only contain letters, digits and _.");
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidAccountException("Password cannot be empty.");

        var passwordHash = _cryptService.Hash(password);
        var account = new Account { Username = username, PasswordHash = passwordHash };
        return await _accountRepository.TryAddAsync(account);
    }

    public async Task<AccountResult> ChangePasswordAsync(Account? account, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return AccountResult.InvalidInput;
        if (account is null)
            return AccountResult.NotFound;

        account.PasswordHash = _cryptService.Hash(newPassword);
        await _accountRepository.UpdateAsync(account);
        return AccountResult.Success;
    }

    public async Task<AccountResult> ChangePasswordAsync(string username, string newPassword)
    {
        var account = await _accountRepository.GetByUsernameAsync(username);
        return await ChangePasswordAsync(account, newPassword);
    }

    public async Task<AccountResult> DeleteAccountAsync(string username)
    {
        // the primary key is now the int Id, so look the account up by username first
        var account = await _accountRepository.GetByUsernameAsync(username);
        if (account is null)
            return AccountResult.NotFound;

        return await _accountRepository.DeleteAsync(account.Id) ? AccountResult.Success : AccountResult.NotFound;
    }
}
