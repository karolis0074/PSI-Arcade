using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

// GetByIdAsync, GetAllAsync, UpdateAsync and DeleteAsync are inherited from Repository<Account>.
// Only the username lookup and the duplicate-checked register stay here.
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
}
