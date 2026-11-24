using SportMatch.API.Models;
using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

public class FakeAuthService : IAuthService
{
    // null = login failed
    public string? Token { get; set; } = "fake-token";

    public bool CheckLoginResult { get; set; } = true;

    public bool VerifyPassword(Account account, string password) => CheckLoginResult;

    public Task<bool> CheckLoginAsync(string username, string password) => Task.FromResult(CheckLoginResult);

    public Task<string?> LoginAsync(string username, string password) => Task.FromResult(Token);
}
