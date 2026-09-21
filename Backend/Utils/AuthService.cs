using Backend;

namespace Backend.Utils
{
    public class AuthService
    {
        private Database _db;
        private TokenService _tokens;

        public AuthService()
        {
            _db = Database.GetInstance();
            _tokens = TokenService.GetInstance();
        }

        public AuthService(Database db, TokenService tokens)
        {
            _db = db;
            _tokens = tokens;
        }

        public bool CheckLogin(Account account, string password)
        {
            if (account == null || password == null)
                return false;

            return account.VerifyPassword(password);
        }

        public bool CheckLogin(string username, string password)
        {
            var account = _db.GetAccount(username);
            return CheckLogin(account, password);
        }

        public string IssueToken(Account account)
        {
            return _tokens.IssueToken(account.Username);
        }

        public Account? AuthenticateToken(string? token)
        {
            var username = _tokens.GetUsername(token);
            if (username == null)
                return null;

            return _db.GetAccount(username);
        }

        public void RevokeToken(string? token)
        {
            _tokens.RevokeToken(token);
        }
    }
}
