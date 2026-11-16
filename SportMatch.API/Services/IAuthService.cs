using SportMatch.API.Models;

namespace SportMatch.API.Services
{
    public interface IAuthService
    {
        public bool CheckLogin(Account account, string password);
        public bool CheckLogin(string username, string password);
        public string? Login(string username, string password);
    }
}
