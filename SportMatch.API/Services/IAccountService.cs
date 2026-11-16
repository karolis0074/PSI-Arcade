using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public interface IAccountService
    {
        public RegisterResult Register(string username, string password);
        public AccountResult ChangePassword(Account? account, string newPassword);
        public AccountResult ChangePassword(string username, string newPassword);
        public AccountResult DeleteAccount(string username);
    }
}
