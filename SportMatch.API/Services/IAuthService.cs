using SportMatch.API.Models;

namespace SportMatch.API.Services
{
    public interface IAuthService
    {
        public bool VerifyPassword(Account account, string password);
        public Task<bool> CheckLoginAsync(string username, string password);
        public Task<string?> LoginAsync(string username, string password);
    }
}
