using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
        Account? GetByUsername(string username);
        RegisterResult TryAdd(Account account);
    }
}
