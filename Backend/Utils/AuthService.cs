using Backend;

namespace SportMatch.API.Utils
{
    public class AuthService
    {
        private Database _db;
        private CryptService _crypt;

        public AuthService()
        {
            _db = Database.GetInstance();
            _crypt = new CryptService();
        }

        public AuthService(Database db)
        {
            _db = db;
            _crypt = new CryptService();
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
