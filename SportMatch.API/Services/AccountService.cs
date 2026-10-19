using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public class AccountService
    {
        private readonly AccountRepository _db;
        private readonly CryptService _crypt;

        public AccountService(AccountRepository db, CryptService crypt)
        {
            _db = db;
            _crypt = crypt;
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
            _db.UpdateAccount(account);
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
            _db.UpdateAccount(account);
            return AccountResult.Success;
        }

        public AccountResult ChangeDisplayName(string username, string newName)
        {
            var account = _db.GetAccount(username);
            return ChangeDisplayName(account, newName);
        }
    }
}