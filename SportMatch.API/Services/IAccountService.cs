using SportMatch.API.Models;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public interface IAccountService
    {
        public Task<RegisterResult> RegisterAsync(string username, string password);
        public Task<AccountResult> ChangePasswordAsync(Account? account, string newPassword);
        public Task<AccountResult> ChangePasswordAsync(string username, string newPassword);
        public Task<AccountResult> DeleteAccountAsync(string username);
    }
}
