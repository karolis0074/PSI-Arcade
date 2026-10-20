using SportMatch.API.Data;
using SportMatch.API.Models;
using System.Collections;
using System.Text.Json;

namespace SportMatch.API.Repositories;

public class SportFieldRepository : IEnumerable<SportField>
{
    private readonly AppDbContext _db;

    public SportFieldRepository(AppDbContext db)
    {
        _db = db;
        SeedIfEmpty();
    }

    private void SeedIfEmpty()
    {
        if (_db.SportFields.Any())
            return;

        using var stream = File.OpenRead("Data/sportfields.json");

        var fields = JsonSerializer.Deserialize<List<SportField>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (fields is not null)
        {
            _db.SportFields.AddRange(fields);
            _db.SaveChanges();
        }
    }

    public IReadOnlyList<SportField> GetAll() => _db.SportFields.ToList();

    public SportField? GetById(int id) =>
        _db.SportFields.FirstOrDefault(f => f.Id == id);

    public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null)
    {
        var query = _db.SportFields.AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(f => f.City.ToLower() == city.ToLower());

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType.ToLower() == gameType.ToLower());

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType.ToLower() == fieldType.ToLower());

        return query.ToList();
    }

    public IEnumerator<SportField> GetEnumerator() =>
        _db.SportFields.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
