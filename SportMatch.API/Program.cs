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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Jwt:Key is not configured. See README-jwt-setup.pdf")))
    };
});
builder.Services.AddAuthorization();

// connection string comes from appsettings.json instead of being hardcoded
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// repositories (scoped: a new one for every request, together with its own DbContext)
builder.Services.AddScoped<ISportFieldRepository, SportFieldRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// services
builder.Services.AddScoped<IGameMembershipService, GameMembershipService>();
builder.Services.AddScoped<IGameManagerService, GameManagerService>();
builder.Services.AddScoped<IGameStatisticsService, GameStatisticsService>();
builder.Services.AddScoped<IGameRecommendationService, GameRecommendationService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<ICryptService, CryptService>();
builder.Services.AddSingleton<IClock, SystemClock>();

// singleton: one shared instance for all requests, so its state must be thread-safe
builder.Services.AddSingleton<ILiveMetricsService, LiveMetricsService>();
builder.Services.AddSingleton<IClock, SystemClock>();

// dependencies of CryptService, injected instead of created with new
builder.Services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
builder.Services.AddSingleton<JwtSecurityTokenHandler>();

var app = builder.Build();

try
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    // SeedIfEmptyAsync() can throw SportFieldDataException
    var sportFields = scope.ServiceProvider.GetRequiredService<ISportFieldRepository>();
    await sportFields.SeedIfEmptyAsync();
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
app.UseMiddleware<ActivityMiddleware>();   // after authentication, so the user is already known
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();