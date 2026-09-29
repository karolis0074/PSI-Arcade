using System.Collections;      
using System.Text.Json;        

namespace Backend.Data;

public class SportFieldStore : IEnumerable<SportField>
{
    private readonly AppDbContext _db;

    public SportFieldStore(AppDbContext db)
    {
        _db = db;
        SeedIfEmpty();   // Įkelia duomenis į DB
    }

    // Įkelia aikšteles iš JSON, jei DB tuščia.
    private void SeedIfEmpty()
    {
        // Jei DB jau turi duomenų - nieko nedarom.
        if (_db.SportFields.Any())
            return;

        using var stream = File.OpenRead("Data/sportfields.json");

        // JSON → List<SportField>.
        // PropertyNameCaseInsensitive - leidžia didžiąsias/mažąsias raides.
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
            query = query.Where(f => f.City == city);

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType == gameType);

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType == fieldType);

        return query.ToList();
    }

    public IEnumerator<SportField> GetEnumerator() =>
        _db.SportFields.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}