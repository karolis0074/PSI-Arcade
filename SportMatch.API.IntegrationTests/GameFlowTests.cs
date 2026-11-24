using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using SportMatch.API.DataTransferObjects.Requests;
using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;

namespace SportMatch.API.IntegrationTests;

public class GameFlowTests
{
    private record LoginResponse(string Token);

    // registers a user and sends their token with every next request
    private static async Task RegisterAndLoginAsync(HttpClient client, string username)
    {
        var register = await client.PostAsJsonAsync("/accounts", new RegisterRequest(username, "password123"));
        register.EnsureSuccessStatusCode();

        var login = await client.PostAsJsonAsync("/accounts/login", new LoginRequest(username, "password123"));
        login.EnsureSuccessStatusCode();

        var body = await login.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body!.Token);
    }

    [Fact]
    public async Task GameAndAccountFlow_SecondPlayer_WorksEndToEnd()
    {
        // arrange
        await using var factory = new SportMatchFactory();
        var host = factory.CreateClient();
        var player = factory.CreateClient();
        await RegisterAndLoginAsync(host, "host");
        await RegisterAndLoginAsync(player, "player");

        // field 1 comes from sportfields.json, the host is joined automatically
        var create = await host.PostAsJsonAsync("/games", new CreateGameRequest(1, DateTime.UtcNow.AddDays(1), 2));
        create.EnsureSuccessStatusCode();
        var game = await create.Content.ReadFromJsonAsync<Game>();

        // act
        var join = await player.PostAsync($"/games/{game!.Id}/players", null);
        var spots = await player.GetFromJsonAsync<GameSpots>($"/games/{game.Id}/spots");
        var statistics = await player.GetFromJsonAsync<GameStatistics>("/games/statistics");
        var leave = await player.DeleteAsync($"/games/{game.Id}/players/me");
        var spotsAfterLeave = await player.GetFromJsonAsync<GameSpots>($"/games/{game.Id}/spots");
        var changePassword = await player.PutAsJsonAsync("/accounts/me/password", new ChangePasswordRequest("password123", "newpass456"));
        var deleteAccount = await player.DeleteAsync("/accounts/me");

        // assert
        Assert.Equal(HttpStatusCode.OK, join.StatusCode);
        Assert.Equal(0, spots.Free);
        Assert.Equal(1, statistics.TotalGames);
        Assert.Equal(HttpStatusCode.OK, leave.StatusCode);
        Assert.Equal(1, spotsAfterLeave.Free);
        Assert.Equal(HttpStatusCode.OK, changePassword.StatusCode);
        Assert.Equal(HttpStatusCode.OK, deleteAccount.StatusCode);
    }
}
