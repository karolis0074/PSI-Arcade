namespace Backend
{
    public class Account
    {
        public String DisplayName { get; private set; }
        public String Username { get; private set; }
        public int Balance { get; private set; }

        private String password; // currently stored raw, will be hashed in the future
        

        public Account(String displayName, String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username) || 
                String.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException("Account username or password is invalid.");

            if (Database.GetInstance().GetAccount(username) != null)
                throw new ArgumentException("Username is taken.");

            this.DisplayName = displayName;
            this.Username = username;
            this.Balance = 5000;

            this.password = password;
        }

        public bool SetDisplayName(String newDisplayName)
        {
            if (String.IsNullOrWhiteSpace(newDisplayName))
                return false;

            DisplayName = newDisplayName;
            return true;
        }

        public int ChangePassword(String oldPassword, String newPassword)
        {
            if (String.IsNullOrWhiteSpace(newPassword))
                return 2; // invalid new password
            if (!CheckPassword(oldPassword))
                return 1; // incorrect current password

            password = newPassword;
            return 0;
        }

        public bool CheckPassword(String password) => password.Equals(this.password);

        public bool TrySpend(int amount)
        {
            if (Balance < amount)
                return false;

            Balance -= amount;
            return true;
        }

        public void AddFunds(int amount) => Balance += amount;
    }
}
