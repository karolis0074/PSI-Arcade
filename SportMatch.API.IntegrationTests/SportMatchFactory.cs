using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace SportMatch.API.IntegrationTests;

// starts the real API (Program.cs) in memory with its own empty database and a test JWT key
public class SportMatchFactory : WebApplicationFactory<Program>
{
    // a separate database file for every factory, so tests never share data
    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"sportmatch-test-{Guid.NewGuid()}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={_databasePath}");
        builder.UseSetting("Jwt:Key", "integration-test-key-that-is-long-enough");
        builder.UseSetting("Jwt:Issuer", "SportMatch.Tests");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        // SQLite keeps closed connections in a pool, so the file has to be released before deleting it
        SqliteConnection.ClearAllPools();
        File.Delete(_databasePath);
    }
}
