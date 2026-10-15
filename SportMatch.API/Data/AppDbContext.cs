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
}