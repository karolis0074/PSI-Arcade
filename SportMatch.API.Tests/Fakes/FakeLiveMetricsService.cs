using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

public class FakeLiveMetricsService : ILiveMetricsService
{
    public void UserConnected(string username) { }

    public void UserDisconnected(string username) { }

    public void Heartbeat(string username) { }

    public int GetActiveUserCount() => 0;
}
