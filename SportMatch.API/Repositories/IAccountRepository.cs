using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

// UpdateAsync, DeleteAsync and the other CRUD methods come from IRepository<Account>
public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByUsernameAsync(string username);
    Task<RegisterResult> TryAddAsync(Account account);
}
