namespace Cella.Core.DataSpaces;

public enum DataSpaceType
{
    FileGroup,
    FileStreamDataFileGroup,
    MemoryOptimizedTablesFileGroup,
    PartitionScheme
}
public abstract class DataSpace
{
    protected DataSpace(int id, string name, DataSpaceType type)
    {
        this.Id = id;
        this.Name = name;
        this.Type = type;
    }
    public string Name { get; set; }
    public int Id { get; }
    public DataSpaceType Type { get; }
    public bool IsDefault { get; set; }
    public bool IsSystem { get; set; }
}

