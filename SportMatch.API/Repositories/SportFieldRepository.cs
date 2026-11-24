using System.Collections;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SportMatch.API.Data;
using SportMatch.API.Exceptions;
using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public class SportFieldRepository : Repository<SportField>, ISportFieldRepository
{
    public SportFieldRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    // a constructor can't be async, so seeding is a separate method called once from Program.cs
    public async Task SeedIfEmptyAsync()
    {
        if (await _dbContext.Set<SportField>().AnyAsync())
            return;

        try
        {
            await using var stream = File.OpenRead("Data/sportfields.json");

            var fields = await JsonSerializer.DeserializeAsync<List<SportField>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (fields is null || fields.Count == 0)
                throw new SportFieldDataException("sportfields.json contains no sport fields.");

            _dbContext.Set<SportField>().AddRange(fields);
            await _dbContext.SaveChangesAsync();
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

    public async Task<IReadOnlyList<SportField>> SearchAsync(string? city = null, string? gameType = null, string? fieldType = null)
    {
        var query = _dbContext.Set<SportField>().AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(f => f.City.ToLower() == city.ToLower());

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType.ToLower() == gameType.ToLower());

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType.ToLower() == fieldType.ToLower());

        return await query.ToListAsync();
    }

    // IEnumerable<T> is synchronous by design (foreach has no await),
    public IEnumerator<SportField> GetEnumerator() =>
        _dbContext.Set<SportField>().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
