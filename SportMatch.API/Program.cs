using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportMatch.API.Data;
using SportMatch.API.Exceptions;
using SportMatch.API.Repositories;
using SportMatch.API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters  // Jwt:Key and Jwt:Issuer have to be set up in user-secrets
    {                                                                  // Look at README-jwt-setup.pdf
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();

// connection string comes from appsettings.json instead of being hardcoded
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// repositories
builder.Services.AddScoped<ISportFieldRepository, SportFieldRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// services
builder.Services.AddScoped<IGameMembershipService, GameMembershipService>();
builder.Services.AddScoped<IGameManagerService, GameManagerService>();
builder.Services.AddScoped<IGameStatisticsService, GameStatisticsService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ICryptService, CryptService>();
builder.Services.AddSingleton<LiveMetricsService>();

// dependencies of CryptService, injected instead of created with new
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddSingleton<JwtSecurityTokenHandler>();

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // creating the repository runs SeedIfEmpty(), which can throw SportFieldDataException
    scope.ServiceProvider.GetRequiredService<ISportFieldRepository>();
}
catch (SportFieldDataException ex)
{
    app.Logger.LogCritical(ex, "Cannot start the API: {Message}", ex.Message);
    return;
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();