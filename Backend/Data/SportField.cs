namespace Backend.Data;

public class SportField
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string GameType { get; set; } = string.Empty;    // Football, Basketball, etc.
    public string FieldType { get; set; } = string.Empty;   // S, M, L
    public int Capacity { get; set; }
    public string? Surface { get; set; }                    // nullable — JSON turi null
    public string WeekdayOpenTime { get; set; } = string.Empty;
    public string WeekdayCloseTime { get; set; } = string.Empty;
    public string WeekendOpenTime { get; set; } = string.Empty;
    public string WeekendCloseTime { get; set; } = string.Empty;
}