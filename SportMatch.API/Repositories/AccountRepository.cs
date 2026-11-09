using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

public class AccountRepository
{
    private readonly AppDbContext _dbContext;

    public AccountRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // username column is NOCASE, so this lookup is case-insensitive
    public Account? GetByUsername(string username) =>
        _dbContext.Accounts.FirstOrDefault(a => a.Username == username);

    public RegisterResult Add(Account account)
    {
        if (account is null)
            return RegisterResult.InvalidInput;
        if (GetByUsername(account.Username) is not null)
            return RegisterResult.UsernameTaken;

        _dbContext.Accounts.Add(account);

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

    public void Update(Account account)
    {
        _dbContext.Accounts.Update(account);
        _dbContext.SaveChanges();
    }

    public AccountResult Delete(string username)
    {
        var account = GetByUsername(username);
        if (account is null)
            return AccountResult.NotFound;

        _dbContext.Accounts.Remove(account);
        _dbContext.SaveChanges();
        return AccountResult.Success;
    }
}
