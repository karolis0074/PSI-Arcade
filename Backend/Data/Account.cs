namespace Backend
{
    public class Account
    {
        public String DisplayName;
        public String Username { get; private set; }
        public int Balance { get; private set; }

        internal String Password; // currently stored raw, will be hashed in the future
        

        public Account(String displayName, String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username) || 
                String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("Account username or password is invalid.");

            this.DisplayName = displayName;
            this.Username = username;
            this.Balance = 5000;

            this.Password = password;
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
