namespace Backend
{
    public class Account
    {
        public String Username { get; private set; }
        public int Balance { get; private set; }

        private String password;
        

        public Account(String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username) || 
                String.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException("Account username or password is invalid.");

            if (Database.GetInstance().GetAccount(username) != null)
                throw new ArgumentException("Username is taken.");

            this.Username = username;
            this.Balance = 5000;

            this.password = password;
        }

        public int SetUsername(String newUsername)
        {
            if (String.IsNullOrWhiteSpace(newUsername))
                return 2;  // invalid username
            if (Database.GetInstance().GetAccount(Username) != null)
                return 1;  // user already exists

            Username = newUsername;
            return 0;
        }

        public int ChangePassword(String oldPassword, String newPassword)
        {
            if (String.IsNullOrWhiteSpace(newPassword))
                return 2; // invalid new password
            if (!oldPassword.Equals(password))
                return 1; // incorrect current password

            password = newPassword;
            return 0;
        }

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
