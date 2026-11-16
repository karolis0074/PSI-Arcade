using SportMatch.API.Models;

namespace SportMatch.API.Repositories
{
    public interface ISportFieldRepository : IEnumerable<SportField>
    {
        public IReadOnlyList<SportField> GetAll();
        public SportField? GetById(int id);
        public IReadOnlyList<SportField> Search(string? city = null, string? gameType = null, string? fieldType = null);

    }
}
