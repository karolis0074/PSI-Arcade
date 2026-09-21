using Backend;

namespace SportMatch.API.Utils
{
    public class AuthService
    {
        private Database _db;

        public AuthService()
        {
            _db = Database.GetInstance();
        }

        public AuthService(Database db)
        {
            _db = db;
        }

        public bool CheckLogin(Account account, string password)
        {
            if (account == null || password == null)
                return false;

            if (password.Equals(account.Password))
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
