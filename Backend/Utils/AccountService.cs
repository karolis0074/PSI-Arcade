using Backend;

namespace SportMatch.API.Utils
{
    public class AccountService
    {
        private Database _db;
        private AuthService _auth;
        private CryptService _crypt;

        public AccountService()
        {
            _db = Database.GetInstance();
            _auth = new AuthService();
            _crypt = new CryptService();
        }

        public AccountService(Database db)
        {
            _db = db;
            _auth = new AuthService(db);
            _crypt = new CryptService();
        }

        public int Register(string displayName, string username, string password)
        {
            if (String.IsNullOrWhiteSpace(displayName) ||
            String.IsNullOrWhiteSpace(username) ||
            String.IsNullOrWhiteSpace(password))
                return 2;

            var passwordHash = _crypt.Hash(password);
            var account = new Account(displayName, username, passwordHash);
            return _db.AddAccount(account);
        }

        public int ChangePassword(Account? account, string newPassword)
        {
            
            if (String.IsNullOrWhiteSpace(newPassword))
                return 2; // bad request
            if (account == null)
                return 1; // not found

            account.PasswordHash = _crypt.Hash(newPassword);
            return 0; // ok
        }

        public int ChangePassword(string username, string newPassword)
        {
            var account = _db.GetAccount(username);
            return ChangePassword(account, newPassword);
        }

        public int ChangeDisplayName(Account account, string newName)
        {
            if (String.IsNullOrWhiteSpace(newName))
                return 2; // bad request
            if (account == null)
                return 1; // account not found

            account.DisplayName = newName;
            return 0;
        }

        public int ChangeDisplayName(string username, string newName)
        {
            var account = _db.GetAccount(username);
            return ChangeDisplayName(account, newName);
        }
    }
}
