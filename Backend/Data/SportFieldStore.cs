using System.Collections;      // IEnumerable (ne-generinis)
using System.Text.Json;        // JsonSerializer

namespace Backend.Data;

// ": IEnumerable<SportField>" - Lab #1 reikalavimas #9.
// Leidžia: foreach (var field in store) { ... }
public class SportFieldStore : IEnumerable<SportField>
{
    // DB kontekstas. Private, readonly.
    private readonly AppDbContext _db;

    // Konstruktorius. DI paduoda AppDbContext.
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

        // Atidaro failą skaitymui (STREAM - Lab #1 reikalavimas #7).
        using var stream = File.OpenRead("Data/sportfields.json");

        // JSON → List<SportField>.
        // PropertyNameCaseInsensitive - leidžia didžiąsias/mažąsias raides.
        var fields = JsonSerializer.Deserialize<List<SportField>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (fields is not null)
        {
            // Prideda į EF sekimo sąrašą (dar ne į DB).
            _db.SportFields.AddRange(fields);

            // Įrašo į DB (šimtai INSERT'ų).
            _db.SaveChanges();
        }
    }

    // Visos aikštelės. ToList() įvykdo SQL.
    public List<SportField> GetAll() => _db.SportFields.ToList();

    // Viena pagal ID. FirstOrDefault - LINQ.
    // "?" - gali būti null, jei nerado.
    public SportField? GetById(int id) =>
        _db.SportFields.FirstOrDefault(f => f.Id == id);

    // Filtravimas pagal 3 parametrus.
    // Visi "string?" - optional (Lab #1 reikalavimas #4).
    public List<SportField> Search(string? city, string? gameType, string? fieldType)
    {
        // AsQueryable - statom užklausą po gabalą, SQL tik ToList().
        var query = _db.SportFields.AsQueryable();

        // !string.IsNullOrEmpty - patikrina, ar ne null IR ne tuščias.
        if (!string.IsNullOrEmpty(city))
            // Where - LINQ filtras (Lab #1 reikalavimas #8).
            query = query.Where(f => f.City == city);

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType == gameType);

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType == fieldType);

        // ToList - įvykdo SQL ir grąžina List.
        return query.ToList();
    }

    // IEnumerable<SportField> reikalavimas.
    // AsEnumerable - konvertuoja DbSet į IEnumerable.
    public IEnumerator<SportField> GetEnumerator() =>
        _db.SportFields.AsEnumerable().GetEnumerator();

    // IEnumerable (ne-generinis) reikalavimas.
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}