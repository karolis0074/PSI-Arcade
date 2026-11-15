using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

// GetById, GetAll, Update and Delete are inherited from Repository<Account>.
// Only the username lookup and the duplicate-checked register stay here.
public class AccountRepository : Repository<Account>
{
    public AccountRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    // username column is NOCASE, so this lookup is case-insensitive
    public Account? GetByUsername(string username) =>
        _set.FirstOrDefault(a => a.Username == username);

    public RegisterResult TryAdd(Account account)
    {
        if (account is null)
            return RegisterResult.InvalidInput;
        if (GetByUsername(account.Username) is not null)
            return RegisterResult.UsernameTaken;

        _set.Add(account);

        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            return RegisterResult.UsernameTaken; // same username was added between the check and the save
        }

        return RegisterResult.Success;
    }
}