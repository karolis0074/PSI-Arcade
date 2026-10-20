using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services
{
    public class AuthService
    {
        private readonly AccountRepository _accountRepo;
        private readonly CryptService _crypt;

        public AuthService(AccountRepository accountRepo, CryptService crypt)
        {
            _accountRepo = accountRepo;
            _crypt = crypt;
        }

        public bool CheckLogin(Account account, string password)
        {
            if (account == null || password == null)
                return false;

            if (_crypt.VerifyHash(password, account.PasswordHash))
                return true;

            return false;
        }

        public bool CheckLogin(string username, string password)
        {
            var account = _accountRepo.GetAccount(username);
            return CheckLogin(account, password);
        }

        public string? Login(string username, string password)
        {
            var account = _accountRepo.GetAccount(username);
            if (account == null || !CheckLogin(account, password))
                return null;

            return _crypt.GenerateToken(account);
        }
    }
}
