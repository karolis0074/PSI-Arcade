namespace Backend.Data;

public readonly struct FootballField
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string City { get; init; }
    public string FieldType { get; init; }
    public int Capacity { get; init; }

    public FootballField(int id, string name, string city, string fieldType, int capacity) // Immutable: field data does not change after creation.
    {
        Id = id;
        Name = name;
        City = city;
        FieldType = fieldType;
        Capacity = capacity;
    }
}