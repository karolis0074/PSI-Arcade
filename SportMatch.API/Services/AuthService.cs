using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services;

public class AuthService : IAuthService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ICryptService _cryptService;

    public AuthService(IAccountRepository accountRepository, ICryptService cryptService)
    {
        _accountRepository = accountRepository;
        _cryptService = cryptService;
    }

    public bool VerifyPassword(Account account, string password)
    {
        if (account is null || password is null)
            return false;

        if (_cryptService.VerifyHash(password, account.PasswordHash))
            return true;

        return false;
    }

    public async Task<bool> CheckLoginAsync(string username, string password)
    {
        var account = await _accountRepository.GetByUsernameAsync(username);
        return VerifyPassword(account, password);
    }

    public async Task<string?> LoginAsync(string username, string password)
    {
        var account = await _accountRepository.GetByUsernameAsync(username);
        if (account is null || !VerifyPassword(account, password))
            return null;

        return _cryptService.GenerateToken(account);
    }
}
