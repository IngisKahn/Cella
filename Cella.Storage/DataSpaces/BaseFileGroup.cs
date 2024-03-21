namespace Cella.Storage.DataSpaces;

public abstract class BaseFileGroup(int id, string name, DataSpaceType type) : DataSpace(id, name, type)
{
    public Guid Guid { get; init; }
    public bool IsReadOnly { get; set; }
    public bool AutoGrowAllFiles { get; set; }
}