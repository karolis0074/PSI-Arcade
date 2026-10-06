using System.Collections;
using System.Text.Json;

namespace Backend.Data;

public class SportFieldStore : IEnumerable<SportField>
{
    private readonly AppDbContext _db;

    public SportFieldStore(AppDbContext db)
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

    public List<SportField> GetAll() => _db.SportFields.ToList();

    public SportField? GetById(int id) =>
        _db.SportFields.FirstOrDefault(f => f.Id == id);

    public List<SportField> Search(string? city, string? gameType, string? fieldType)
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