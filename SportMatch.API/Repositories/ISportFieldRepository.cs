using SportMatch.API.Models;

namespace SportMatch.API.Repositories;

public interface ISportFieldRepository : IRepository<SportField>, IEnumerable<SportField>
{
    Task SeedIfEmptyAsync();
    Task<IReadOnlyList<SportField>> SearchAsync(string? city = null, string? gameType = null, string? fieldType = null);
}
