namespace SportMatch.API.Services;

// real clock used by the app
public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
