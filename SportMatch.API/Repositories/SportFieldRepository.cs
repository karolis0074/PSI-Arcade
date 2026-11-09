using SportMatch.API.Data;
using SportMatch.API.Models;
using System.Collections;
using System.Text.Json;
using SportMatch.API.Exceptions;

namespace SportMatch.API.Repositories;

// GetAll and GetById are inherited from Repository<SportField>, only field-specific logic stays here
public class SportFieldRepository : Repository<SportField>, IEnumerable<SportField>
{
    public SportFieldRepository(AppDbContext dbContext) : base(dbContext)
    {
        SeedIfEmpty();
    }

    private void SeedIfEmpty()
    {
        if (_set.Any())
            return;

        try
        {
            using var stream = File.OpenRead("Data/sportfields.json");

            var fields = JsonSerializer.Deserialize<List<SportField>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (fields is null || fields.Count == 0)
                throw new SportFieldDataException("sportfields.json contains no sport fields.");

            _set.AddRange(fields);
            _dbContext.SaveChanges();
        }
        catch (FileNotFoundException ex)
        {
            throw new SportFieldDataException("sportfields.json was not found.", ex);
        }
        catch (JsonException ex)
        {
            throw new SportFieldDataException("sportfields.json is not valid JSON.", ex);
        }
    }

    public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null)
    {
        var query = _set.AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(f => f.City.ToLower() == city.ToLower());

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType.ToLower() == gameType.ToLower());

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType.ToLower() == fieldType.ToLower());

        return query.ToList();
    }

    public IEnumerator<SportField> GetEnumerator() =>
        _set.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}