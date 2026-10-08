using Backend;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportMatch.API.Utils
{
    public class CryptService
    {
        private readonly PasswordHasher<object> _hasher = new(); // the TUser type param is not used in the default methods, so safe to pass null! below
        private readonly SigningCredentials _signingCreds;
        private readonly string _issuer;

        public CryptService()
        {
            var config = new ConfigurationBuilder().AddUserSecrets<CryptService>().Build();

            string jwtKey = config["Jwt:Key"];
            string jwtIssuer = config["Jwt:Issuer"];
            if (jwtKey == null || jwtIssuer == null)
                throw new InvalidOperationException("Jwt:Key and Jwt:Issuer must be configured. Please take a look at README-jwt-setup.pdf");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)); // Jwt:Key and Jwt:Issuer have to be set up
            _signingCreds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);     // Look at README-jwt-setup.pdf
            _issuer = config["Jwt:Issuer"]!;
        }

        public string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }

        public bool VerifyHash(string password, string hash)
        {
            var result = _hasher.VerifyHashedPassword(null!, hash, password);
            return result == PasswordVerificationResult.Success;
        }

        public string GenerateToken(Account account)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, account.Username) };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: _signingCreds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
