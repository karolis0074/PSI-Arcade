using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services;

public class AuthService
{
    private readonly AccountRepository _accountRepository;
    private readonly CryptService _cryptService;

    public AuthService(AccountRepository accountRepository, CryptService cryptService)
    {
        _accountRepository = accountRepository;
        _cryptService = cryptService;
    }

    public bool CheckLogin(Account account, string password)
    {
        if (account is null || password is null)
            return false;

        if (_cryptService.VerifyHash(password, account.PasswordHash))
            return true;

        return false;
    }

    public bool CheckLogin(string username, string password)
    {
        var account = _accountRepository.GetByUsername(username);
        return CheckLogin(account, password);
    }

    public string? Login(string username, string password)
    {
        var account = _accountRepository.GetByUsername(username);
        if (account is null || !CheckLogin(account, password))
            return null;

        return _cryptService.GenerateToken(account);
    }
}
