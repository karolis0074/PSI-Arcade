using Microsoft.AspNetCore.Identity;

namespace SportMatch.API.Models
{
    public class Account
    {
        public string DisplayName;
        public string Username { get; private set; }
        public int Balance { get; private set; }

        internal string PasswordHash;
        

        public Account(string displayName, string username, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(username) || 
                string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("Account username or password is invalid.");

            DisplayName = displayName;
            Username = username;
            Balance = 5000;

            PasswordHash = passwordHash;
        }

        public bool TrySpend(int amount)
        {
            if (Balance < amount || amount < 0)
                return false;

            Balance -= amount;
            return true;
        }

        public bool AddFunds(int amount)
        {
            if (amount < 0)
                return false;

            Balance += amount;
            return true;
        }      
    }
}
