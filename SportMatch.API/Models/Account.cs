using System.Text.Json.Serialization;

namespace SportMatch.API.Models
{
    public class Account
    {
        public string DisplayName { get; set; }
        public string Username { get; private set; }

        [JsonIgnore] public string PasswordHash { get; set; }
        

        public Account(string displayName, string username, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("Account username or password is invalid.");

            DisplayName = displayName;
            Username = username;

            PasswordHash = passwordHash;
        }
    }
}
