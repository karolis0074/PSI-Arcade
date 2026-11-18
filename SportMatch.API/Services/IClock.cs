namespace SportMatch.API.Services;

// the current time, behind an interface so tests can pass a fake clock
public interface IClock
{
    DateTime UtcNow { get; }
}
