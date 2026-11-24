using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SportMatch.API.DataTransferObjects.Requests;

namespace SportMatch.API.IntegrationTests;

public class AccountFlowTests
{
    private record LoginResponse(string Token);

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client, string username, string password)
        => client.PostAsJsonAsync("/accounts/login", new LoginRequest(username, password));

    // takes the token from a login response and sends it with every next request
    private static async Task UseTokenAsync(HttpClient client, HttpResponseMessage login)
    {
        var body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task Register_TakenOrInvalidUsername_ReturnsError()
    {
        // arrange
        await using var factory = new SportMatchFactory();
        var client = factory.CreateClient();
        var first = await client.PostAsJsonAsync("/accounts", new RegisterRequest("jonas", "password123"));

        // act
        var sameName = await client.PostAsJsonAsync("/accounts", new RegisterRequest("jonas", "password123"));
        var badName = await client.PostAsJsonAsync("/accounts", new RegisterRequest("jonas!", "password123"));

        // assert
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, sameName.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, badName.StatusCode);
    }

    [Fact]
    public async Task AccountLifecycle_ChangePasswordLogoutAndDelete_WorksEndToEnd()
    {
        // arrange
        await using var factory = new SportMatchFactory();
        var client = factory.CreateClient();
        var register = await client.PostAsJsonAsync("/accounts", new RegisterRequest("jonas", "password123"));
        register.EnsureSuccessStatusCode();

        // act
        var wrongPassword = await LoginAsync(client, "jonas", "wrongpass");
        var login = await LoginAsync(client, "jonas", "password123");
        await UseTokenAsync(client, login);

        var change = await client.PutAsJsonAsync("/accounts/me/password", new ChangePasswordRequest("password123", "newpass456"));
        var logout = await client.PostAsync("/accounts/logout", null);

        var oldPassword = await LoginAsync(client, "jonas", "password123");
        var newPassword = await LoginAsync(client, "jonas", "newpass456");
        await UseTokenAsync(client, newPassword);

        var delete = await client.DeleteAsync("/accounts/me");
        var afterDelete = await LoginAsync(client, "jonas", "newpass456");

        // assert
        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.OK, change.StatusCode);
        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, oldPassword.StatusCode);
        Assert.Equal(HttpStatusCode.OK, newPassword.StatusCode);
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, afterDelete.StatusCode);
    }
}
