using System.Security.Cryptography;

namespace Backend.Utils
{
    public class TokenService // temporary in-memory session store, to be replaced with a real store later on
    {
        private Dictionary<string, string> tokensToUsernames;

        private static TokenService instance;

        public TokenService()
        {
            tokensToUsernames = new Dictionary<string, string>();
        }

        public static TokenService GetInstance()
        {
            if (instance == null)
                instance = new TokenService();
            return instance;
        }

        public string IssueToken(string username)
        {
            string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            tokensToUsernames[token] = username;
            return token;
        }

        public string? GetUsername(string? token)
        {
            if (String.IsNullOrWhiteSpace(token))
                return null;

            return tokensToUsernames.TryGetValue(token, out var username) ? username : null;
        }

        public void RevokeToken(string? token)
        {
            if (token != null)
                tokensToUsernames.Remove(token);
        }
    }
}
