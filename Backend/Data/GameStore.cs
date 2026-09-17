namespace Backend.Data;

public class GameStore
{
    // Static storage so data persists across requests.
    // Temporary solution until DB is added.
    private static readonly List<Game> _games = new();
    private static int _nextId = 1;

    public Game Add(Game game)
    {
        game.Id = _nextId++;
        _games.Add(game);
        return game;
    }

    public Game? GetById(int id)
    {
        return _games.FirstOrDefault(g => g.Id == id);
    }

    public List<Game> GetAll()
    {
        return _games;
    }

    public bool Delete(int id)
    {
        var game = GetById(id);
        if (game is null)
            return false;

        _games.Remove(game);
        return true;
    }
}