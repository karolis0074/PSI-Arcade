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

            this.Username = username;
            this.Balance = 5000;

            this.password = password;
            
        }

        public bool SetUsername(String username)
        {
            if (String.IsNullOrWhiteSpace(username))
                return false;

            this.Username = username;
            return true;
        }

        public bool ChangePassword(String oldPassword, String newPassword)
        {
            if (String.IsNullOrWhiteSpace(newPassword) || !oldPassword.Equals(newPassword))
                return false;
            password = newPassword;
            return true;
        }
    }
}
