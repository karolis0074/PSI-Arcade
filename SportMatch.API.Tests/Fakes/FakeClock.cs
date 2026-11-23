using SportMatch.API.Services;

namespace SportMatch.API.Tests.Fakes;

// fake clock: the test decides what "now" is, so results never depend on the real time
public class FakeClock : IClock
{
    public DateTime UtcNow { get; set; } = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
