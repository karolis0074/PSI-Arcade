using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Utils
{
    public class AccountService
    {
        private AccountRepository _db;
        private AuthService _auth;
        private CryptService _crypt;

        public AccountService()
        {
            _db = AccountRepository.GetInstance();
            _auth = new AuthService();
            _crypt = new CryptService();
        }

        public AccountService(AccountRepository db)
        {
            _db = db;
            _auth = new AuthService(db);
            _crypt = new CryptService();
        }

        public RegisterResult Register(string displayName, string username, string password)
        {
            if (String.IsNullOrWhiteSpace(displayName) ||
                String.IsNullOrWhiteSpace(username) ||
                String.IsNullOrWhiteSpace(password))
                return RegisterResult.InvalidInput;

            var passwordHash = _crypt.Hash(password);
            var account = new Account(displayName, username, passwordHash);
            return _db.AddAccount(account);
        }

        public AccountResult ChangePassword(Account? account, string newPassword)
        {
            if (String.IsNullOrWhiteSpace(newPassword))
                return AccountResult.InvalidInput;
            if (account == null)
                return AccountResult.NotFound;

            account.PasswordHash = _crypt.Hash(newPassword);
            return AccountResult.Success;
        }

        public AccountResult ChangePassword(string username, string newPassword)
        {
            var account = _db.GetAccount(username);
            return ChangePassword(account, newPassword);
        }

        public AccountResult ChangeDisplayName(Account? account, string newName)
        {
            if (String.IsNullOrWhiteSpace(newName))
                return AccountResult.InvalidInput;
            if (account == null)
                return AccountResult.NotFound;

            account.DisplayName = newName;
            return AccountResult.Success;
        }

        public AccountResult ChangeDisplayName(string username, string newName)
        {
            var account = _db.GetAccount(username);
            return ChangeDisplayName(account, newName);
        }
    }
}