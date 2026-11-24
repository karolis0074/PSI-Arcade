using SportMatch.API.Data;
using SportMatch.API.Models;
using System.Collections;
using System.Text.Json;
using SportMatch.API.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SportMatch.API.Repositories;

public class SportFieldRepository : Repository<SportField>, ISportFieldRepository
{
    public SportFieldRepository(AppDbContext dbContext) : base(dbContext)
    {
        SeedIfEmpty();
    }

    private void SeedIfEmpty()
    {
        if (_dbContext.Set<SportField>().Any())
            return;

        try
        {
            using var stream = File.OpenRead("Data/sportfields.json");

            var fields = JsonSerializer.Deserialize<List<SportField>>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (fields is null || fields.Count == 0)
                throw new SportFieldDataException("sportfields.json contains no sport fields.");

            _dbContext.Set<SportField>().AddRange(fields);
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

    public async Task<IReadOnlyList<SportField>> GetAllAsync() => 
        await _dbContext.SportFields.ToListAsync();

    public async Task<SportField?> GetByIdAsync(int id) => 
        await _dbContext.SportFields.FirstOrDefaultAsync(f => f.Id == id);

    public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null)
    {
        var query = _dbContext.Set<SportField>().AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(f => f.City.ToLower() == city.ToLower());

        if (!string.IsNullOrEmpty(gameType))
            query = query.Where(f => f.GameType.ToLower() == gameType.ToLower());

        if (!string.IsNullOrEmpty(fieldType))
            query = query.Where(f => f.FieldType.ToLower() == fieldType.ToLower());

        return query.ToList();
    }

    public IEnumerator<SportField> GetEnumerator() =>
        _dbContext.Set<SportField>().AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}