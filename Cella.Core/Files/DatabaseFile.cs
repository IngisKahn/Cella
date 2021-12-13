namespace Cella.Core;

public enum AutoGrowthType
{
    ByPercent,
    ByExtent
}

public enum DatabaseFileType
{
    Pages,
    Log,
    FileStream,
    Fixed
}

public enum DatabaseFileState
{
    Online,
    Restoring,
    Recovering,
    RecoveryPending,
    Suspect,
    Offline,
    Defunct
}

public class DatabaseFile : IDatabaseFile
{
    public ushort Id { get; }
    public Guid Guid { get; }
    public string Name { get; }
    public string PhysicalName { get; }
    public DatabaseFileType Type { get; }
    public FileGroup FileGroup { get; }
    public DatabaseFileState State { get; set; } = DatabaseFileState.Offline;
    public bool IsMediaReadOnly { get; init; }
    public bool IsReadOnly { get; set; }
    public bool IsSparse { get; init; }
    public bool IsNameReserved { get; set; }

    public DatabaseFile(FileGroup fileGroup, ushort id, string name, string physicalName, DatabaseFileType type)
        : this(fileGroup, id, name, physicalName, type, Guid.NewGuid()) { }
    public DatabaseFile(FileGroup fileGroup, ushort id, string name, string physicalName, DatabaseFileType type, Guid guid)
    {
        this.FileGroup = fileGroup;
        this.Id = id;
        this.Name = name;
        this.PhysicalName = physicalName;
        this.Guid = guid;
        this.Type = type;
    }

    public virtual void Validate()
    {
        if (this.Type != DatabaseFileType.FileStream)
            throw new CellaException("Only unmanaged data files can be file streams");
        if (Path.GetExtension(this.PhysicalName).Length != 0)
            throw new CellaException($"The path {this.PhysicalName} for FileStream {this.Name} must be a directory");
        var parentFolder = Path.GetRelativePath(this.PhysicalName, "..");
        if (!Directory.Exists(parentFolder))
            throw new CellaException($"The path {parentFolder} for FileStream {this.Name} does not exist");
        if (Directory.Exists(this.PhysicalName))
            throw new CellaException($"The path {this.PhysicalName} for FileStream {this.Name} must not already exist");
    }

    public virtual void Create()
    {
        this.Validate();
        Directory.CreateDirectory(this.PhysicalName);
    }
}
