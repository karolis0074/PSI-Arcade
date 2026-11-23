using System.Collections.Concurrent;

namespace SportMatch.API.Services;

public class LiveMetricsService : ILiveMetricsService
{
    // Saugome vartotojo vardą ir laiką, kada jis paskutinį kartą buvo aktyvus
    private readonly ConcurrentDictionary<string, DateTime> _activeUsers = new();

    public void UserConnected(string username)
    {
        // Atnaujiname arba pridedame vartotoją su dabartiniu laiku
        _activeUsers[username] = DateTime.UtcNow;
    }

    public void UserDisconnected(string username)
    {
        // Ištriname vartotoją
        _activeUsers.TryRemove(username, out _);
    }

    public int GetActiveUserCount()
    {
        // Išvalome vartotojus, kurie neaktyvūs ilgiau nei 5 minutes
        var threshold = DateTime.UtcNow.AddMinutes(-5);
        
        foreach (var user in _activeUsers)
        {
            if (user.Value < threshold)
            {
                _activeUsers.TryRemove(user.Key, out _);
            }
        }

        return _activeUsers.Count;
    }
}