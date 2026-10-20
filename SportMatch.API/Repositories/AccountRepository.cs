using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

public class AccountRepository
{
    private readonly AppDbContext _db;

    public AccountRepository(AppDbContext db)
    {
        _db = db;
    }

    // username column is NOCASE, so this lookup is case-insensitive
    public Account? GetByUsername(string username) =>
        _db.Accounts.FirstOrDefault(a => a.Username == username);

    public RegisterResult Add(Account account)
    {
        if (account is null)
            return RegisterResult.InvalidInput;
        if (GetByUsername(account.Username) is not null)
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

    public void Update(Account account)
    {
        _db.Accounts.Update(account);
        _db.SaveChanges();
    }

    public AccountResult Delete(string username)
    {
        var account = GetByUsername(username);
        if (account is null)
            return AccountResult.NotFound;

        _db.Accounts.Remove(account);
        _db.SaveChanges();
        return AccountResult.Success;
    }
}
