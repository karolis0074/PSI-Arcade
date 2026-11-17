namespace SportMatch.API.Services;

public class LiveMetricsService
{
    private int _activeUsers = 0;

    public void UserConnected()
    {
        Interlocked.Increment(ref _activeUsers);
    }

    public void UserDisconnected()
    {
        Interlocked.Decrement(ref _activeUsers);
    }

    public int GetActiveUserCount()
    {
        return _activeUsers;
    }
}