using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories
{
    public class AccountRepository
    {
        private readonly AppDbContext _db;

        public AccountRepository(AppDbContext db)
        {
            _db = db;
        }

        // Username column is NOCASE, so this lookup is case-insensitive
        public Account? GetAccount(string username)
        {
            return _db.Accounts.FirstOrDefault(a => a.Username == username);
        }

        public RegisterResult AddAccount(Account account)
        {
            if (account == null)
                return RegisterResult.InvalidInput;
            if (GetAccount(account.Username) != null)
                return RegisterResult.UsernameTaken;

            _db.Accounts.Add(account);

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
            {
                return RegisterResult.UsernameTaken; // same username was added between the check and the save
            }

            return RegisterResult.Success;
        }

        public void UpdateAccount(Account account)
        {
            _db.Accounts.Update(account);
            _db.SaveChanges();
        }

        public AccountResult DeleteAccount(string username)
        {
            Account? account = GetAccount(username);
            if (account == null)
                return AccountResult.NotFound;

            _db.Accounts.Remove(account);
            _db.SaveChanges();
            return AccountResult.Success;
        }
    }
}