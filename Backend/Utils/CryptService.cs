using Backend;
using Microsoft.AspNetCore.Identity;

namespace SportMatch.API.Utils
{
    public class CryptService
    {
        private readonly PasswordHasher<object> _hasher = new(); // the TUser type param is not used in the default methods, so safe to pass null! below

        public string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool VerifyHash(string password, string hash)
        {
            var result = _hasher.VerifyHashedPassword(null!, hash, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
