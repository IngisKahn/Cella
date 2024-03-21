namespace Cella.Storage.DataSpaces;

public abstract class DataSpace(int id, string name, DataSpaceType type)
{
    public string Name { get; } = name;
    public int Id { get; } = id;
    public DataSpaceType Type { get; } = type;
    public bool IsDefault { get; protected init; }
    public bool IsSystem { get; set; }
}

