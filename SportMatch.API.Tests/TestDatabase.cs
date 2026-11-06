using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Models;

namespace SportMatch.API.Tests;

// makes an empty database in memory, so tests don't touch app.db
public static class TestDatabase
{
    public static AppDbContext Create()
    {
        // in memory database lives only while the connection is open
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new AppDbContext(options);
        dbContext.Database.EnsureCreated();

        // every game needs a field, because Games has a foreign key to SportFields
        dbContext.SportFields.Add(new SportField { Id = 1, Name = "Test field", City = "Vilnius", Capacity = 10 });
        dbContext.SaveChanges();

        return dbContext;
    }
}
