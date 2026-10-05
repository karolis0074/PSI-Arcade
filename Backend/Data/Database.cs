using Backend.Data;

namespace Backend
{
    public class Database // temporary singleton that stores user data while running
    {                     // to be replaced with sql server or some other legitimate db later on
        private List<Account> accounts;

        private static Database instance;

        private Database()
        {
            accounts = new List<Account>();
        }

        public static Database GetInstance()
        {
            if (instance == null)
                instance = new Database();
            return instance;
        }

        public Account? GetAccount(string username)
        {
            return accounts.FirstOrDefault(a =>
                String.Equals(a.Username, username, StringComparison.OrdinalIgnoreCase));
        }

        public RegisterResult AddAccount(Account account)
        {
            if (account == null)
                return RegisterResult.InvalidInput;
            if (GetAccount(account.Username) != null)
                return RegisterResult.UsernameTaken;

            accounts.Add(account);
            return RegisterResult.Success;
        }

        public AccountResult DeleteAccount(string username)
        {
            Account? account = GetAccount(username);
            if (account == null || !accounts.Remove(account))
                return AccountResult.NotFound;

            return AccountResult.Success;
        }
    }
}