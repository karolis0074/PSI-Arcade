using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Repositories
{
    public interface IAccountRepository
    {
        public Account? GetByUsername(string username);
        public RegisterResult Add(Account account);
        public void Update(Account account);
        public AccountResult Delete(string username);
    }
}
