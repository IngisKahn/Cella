namespace Cella.Storage.DataSpaces;

public abstract class BaseFileGroup : DataSpace
{
    public Guid Guid { get; init; }
    public bool IsReadOnly { get; set; }
    public bool AutoGrowAllFiles { get; set; }

    protected BaseFileGroup(int id, string name, DataSpaceType type) : base(id, name, type) { }
}