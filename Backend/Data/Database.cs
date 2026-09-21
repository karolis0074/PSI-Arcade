using System.Collections;
using System.Runtime.InteropServices;

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
            foreach (Account account in accounts)
            {
                if (String.Equals(account.Username, username, StringComparison.OrdinalIgnoreCase))
                    return account;
            }
            return null;
        }

        public int AddAccount(Account account)
        {
            if (account == null)
                return 2;
            if (GetAccount(account.Username) != null)
                return 1;

            accounts.Add(account);
            return 0;
        }

        public int DeleteAccount(string username)
        {
            Account account = GetAccount(username);
            if (account == null || !accounts.Remove(account))
                return 1;

            return 0;
        }
    }
}
