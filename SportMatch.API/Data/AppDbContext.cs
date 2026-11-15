using Microsoft.EntityFrameworkCore;
using SportMatch.API.Models;

namespace SportMatch.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<SportField> SportFields { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            // Id is the primary key by convention, the username only has to stay unique (case-insensitive)
            entity.Property(a => a.Username).UseCollation("NOCASE");
            entity.HasIndex(a => a.Username).IsUnique();
        });
    }
}
