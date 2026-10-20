using SportMatch.API.Models;
using SportMatch.API.Repositories;

namespace SportMatch.API.Services
{
    public class AuthService
    {
        private readonly AccountRepository _db;
        private readonly CryptService _crypt;

        public AuthService(AccountRepository db, CryptService crypt)
        {
            _db = db;
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
            var account = _db.GetAccount(username);
            return CheckLogin(account, password);
        }
    }
}
