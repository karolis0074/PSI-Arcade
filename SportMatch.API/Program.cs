using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SportMatch.API.Data;
using SportMatch.API.Repositories;
using SportMatch.API.Services;
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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddScoped<SportFieldRepository>();
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<GameMembershipService>();
builder.Services.AddScoped<GameManagerService>();
builder.Services.AddScoped<GameStatisticsService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<CryptService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    var sportFieldRepository = scope.ServiceProvider.GetRequiredService<SportFieldRepository>();
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
