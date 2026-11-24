using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByUsernameAsync(string username);
    Task<RegisterResult> TryAddAsync(Account account);
}
