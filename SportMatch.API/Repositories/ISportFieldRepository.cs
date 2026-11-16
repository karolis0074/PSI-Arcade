using SportMatch.API.Models;

namespace SportMatch.API.Repositories
{
    public interface ISportFieldRepository : IRepository<SportField>, IEnumerable<SportField>
    {
        IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null);
    }
}
