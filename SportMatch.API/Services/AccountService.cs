using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

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

    public RegisterResult Register(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
            return RegisterResult.InvalidInput;

        var passwordHash = _cryptService.Hash(password);
        var account = new Account { Username = username, PasswordHash = passwordHash };
        return _accountRepository.Add(account);
    }

    public AccountResult ChangePassword(Account? account, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            return AccountResult.InvalidInput;
        if (account is null)
            return AccountResult.NotFound;

        account.PasswordHash = _cryptService.Hash(newPassword);
        _accountRepository.Update(account);
        return AccountResult.Success;
    }

    public AccountResult ChangePassword(string username, string newPassword)
    {
        var account = _accountRepository.GetByUsername(username);
        return ChangePassword(account, newPassword);
    }

    public AccountResult DeleteAccount(string username) => _accountRepository.Delete(username);
}
