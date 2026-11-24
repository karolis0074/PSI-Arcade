using System.Collections.Concurrent;

namespace SportMatch.API.Services;

public class LiveMetricsService : ILiveMetricsService
{
    private readonly ConcurrentDictionary<string, DateTime> _activeUsers = new();

    public void UserConnected(string username)
    {
        _activeUsers[username] = DateTime.UtcNow;
    }

    public void UserDisconnected(string username)
    {
        _activeUsers.TryRemove(username, out _);
    }

    public void Heartbeat(string username)
    {
        if (!string.IsNullOrEmpty(username) && _activeUsers.ContainsKey(username))
        {
            _activeUsers[username] = DateTime.UtcNow;
        }
    }

    public int GetActiveUserCount()
    {
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