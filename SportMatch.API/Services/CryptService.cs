using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SportMatch.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportMatch.API.Services;

public class CryptService
{
    private readonly IPasswordHasher<object> _hasher;          // the TUser type param is not used in the default methods, so safe to pass null! below
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly SigningCredentials _signingCredentials;
    private readonly string _issuer;

    // all dependencies come from DI, nothing is created manually
    public CryptService(IConfiguration config, IPasswordHasher<object> hasher, JwtSecurityTokenHandler tokenHandler)
    {
        _hasher = hasher;
        _tokenHandler = tokenHandler;

        string? jwtKey = config["Jwt:Key"];         // Jwt:Key and Jwt:Issuer have to be set up in user-secrets
        string? jwtIssuer = config["Jwt:Issuer"];   // Look at README-jwt-setup.pdf
        if (jwtKey is null || jwtIssuer is null)
            throw new InvalidOperationException("Jwt:Key and Jwt:Issuer must be configured. Please take a look at README-jwt-setup.pdf");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        _issuer = jwtIssuer;
    }

    public string Hash(string password) => _hasher.HashPassword(null!, password);

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
            signingCredentials: _signingCredentials);

        return _tokenHandler.WriteToken(token);
    }
}