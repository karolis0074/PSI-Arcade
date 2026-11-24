using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

// sync GetById, GetAll, Update and Delete are inherited from Repository<Account>.
// The async methods used by the account services are here.
public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    // username column is NOCASE, so this lookup is case-insensitive
    public async Task<Account?> GetByUsernameAsync(string username) =>
        await _dbContext.Set<Account>().FirstOrDefaultAsync(a => a.Username == username);

    public async Task<RegisterResult> TryAddAsync(Account account)
    {
        if (account is null)
            return RegisterResult.InvalidInput;
        if (await GetByUsernameAsync(account.Username) is not null)
            return RegisterResult.UsernameTaken;

        _dbContext.Set<Account>().Add(account);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            return RegisterResult.UsernameTaken; // same username was added between the check and the save
        }

        return RegisterResult.Success;
    }

    public async Task UpdateAsync(Account account)
    {
        _dbContext.Set<Account>().Update(account);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var account = await _dbContext.Set<Account>().FindAsync(id);
        if (account is null)
            return false;

        _dbContext.Set<Account>().Remove(account);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}