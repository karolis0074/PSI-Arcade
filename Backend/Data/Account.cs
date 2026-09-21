using Backend.Utils;

namespace Backend
{
    public class Account
    {
        public String DisplayName;
        public String Username { get; private set; }
        public int Balance { get; private set; }

        internal String PasswordHash { get; private set; }

        public Account(String displayName, String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username) ||
                String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("Account username or password is invalid.");

            this.DisplayName = displayName;
            this.Username = username;
            this.Balance = 5000;

            this.PasswordHash = PasswordHasher.Hash(password);
        }

        public bool VerifyPassword(String password)
        {
            return PasswordHasher.Verify(password, PasswordHash);
        }

        public void SetPassword(String password)
        {
            PasswordHash = PasswordHasher.Hash(password);
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
