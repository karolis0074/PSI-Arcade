using SportMatch.API.Models;

namespace SportMatch.API.Services
{
    public interface ICryptService
    {
        public string Hash(string password);
        public bool VerifyHash(string password, string hash);
        public string GenerateToken(Account account);
    }
}
