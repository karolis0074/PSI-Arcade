using Backend.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// DB setup
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Store registracija
builder.Services.AddScoped<SportFieldStore>();

var app = builder.Build();

// Automatinė migracija ir seed'inimas paleidžiant programą
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Seed'inimas — sukuria SportFieldStore, kuris įkelia duomenis
    var store = scope.ServiceProvider.GetRequiredService<SportFieldStore>();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();