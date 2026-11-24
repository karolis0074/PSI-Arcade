using System.Collections.Concurrent;

namespace SportMatch.API.Services;

// registered as a singleton, so every request shares this one object.
// many requests run on different threads at the same time, so all state lives
public class LiveMetricsService : ILiveMetricsService
{
    private static readonly TimeSpan InactiveAfter = TimeSpan.FromMinutes(5);

    // usernames are case-insensitive everywhere in the app, so here too
    private readonly ConcurrentDictionary<string, DateTime> _activeUsers =
        new(StringComparer.OrdinalIgnoreCase);

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
        if (string.IsNullOrEmpty(username))
            return;

        // TryUpdate only writes if the user is still there with the value we read,
        // so a user who logged out in the meantime is not added back
        if (_activeUsers.TryGetValue(username, out var lastSeen))
            _activeUsers.TryUpdate(username, DateTime.UtcNow, lastSeen);
    }

    public int GetActiveUserCount()
    {
        var threshold = DateTime.UtcNow - InactiveAfter;

        foreach (var user in _activeUsers)
        {
            // removes the pair only if its time hasn't changed since we read it,
            // so a user whose Heartbeat just ran is not removed by mistake
            if (user.Value < threshold)
                _activeUsers.TryRemove(user);
        }

        return _activeUsers.Count;
    }
}