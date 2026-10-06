namespace Backend.Data;

public class GameStore
{
    private readonly AppDbContext _db;

    public GameStore(AppDbContext db)
    {
        _db = db;
    }

    public Game Add(Game game)
    {
        _db.Games.Add(game);
        _db.SaveChanges();
        return game;
    }

    public Game? GetById(int id) =>
        _db.Games.FirstOrDefault(g => g.Id == id);

    public List<Game> GetAll() => _db.Games.ToList();

    public bool Delete(int id)
    {
        var game = GetById(id);
        if (game is null) return false;

        _db.Games.Remove(game);
        _db.SaveChanges();
        return true;
    }
}