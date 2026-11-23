namespace SportMatch.API.Services;

public interface ILiveMetricsService
{
    void UserConnected(string username);
    void UserDisconnected(string username);
    void Heartbeat(string username);
    int GetActiveUserCount();
}