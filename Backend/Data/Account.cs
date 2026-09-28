using Microsoft.AspNetCore.Identity;

namespace Backend
{
    public class Account
    {
        public String DisplayName;
        public String Username { get; private set; }
        public int Balance { get; private set; }

        internal String PasswordHash;
        

        public Account(String displayName, String username, String passwordHash)
        {
            if (String.IsNullOrWhiteSpace(username) || 
                String.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("Account username or password is invalid.");

            this.DisplayName = displayName;
            this.Username = username;
            this.Balance = 5000;

            this.PasswordHash = passwordHash;
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
