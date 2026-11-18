namespace SportMatch.API.Models;

// common base for every entity stored through Repository<T>.
// abstract, so it can't be created on its own and EF Core doesn't map it as a table.
public abstract class BaseEntity
{
    public int Id { get; set; }
}
