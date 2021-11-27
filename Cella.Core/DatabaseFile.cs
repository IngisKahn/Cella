using System.Collections.Concurrent;

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

public class DatabaseFile
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

public class ManagedFile : DatabaseFile
{
    public ManagedFile(FileGroup fileGroup, ushort id, string name, string physicalName, DatabaseFileType type) 
        : base(fileGroup, id, name, physicalName, type)
    {
    }

    public ManagedFile(FileGroup fileGroup, ushort id, string name, string physicalName, Guid guid, DatabaseFileType type) 
        : base(fileGroup, id, name, physicalName, type, guid)
    {
    }

    public int InitialSize { get; init; } = 48;
    public int Size { get; set; }
    public AutoGrowthType AutoGrowthType { get; init; } = AutoGrowthType.ByExtent;
    public uint AutoGrowthAmount { get; init; } = 16;
    public int MaximumSize { get; init; } = -1;

    public override void Validate()
    {
        if (this.Type == DatabaseFileType.FileStream)
            throw new CellaException("Only unmanaged data files can be file streams");
        var path = Path.GetFullPath(this.PhysicalName);
        if (!Directory.Exists(path))
            throw new CellaException($"The path {this.PhysicalName} for {this.Name} does not exist");
        if (File.Exists(this.PhysicalName))
            throw new CellaException($"The file {this.PhysicalName} for {this.Name} already exists");
    }

    public override void Create()
    {
        using var fileStream = File.Create(this.PhysicalName);
        fileStream.SetLength(this.InitialSize);
    }

    public void Grow()
    {
        using var fileStream = File.Open(this.PhysicalName, FileMode.Open, FileAccess.Write);
        var size = this.AutoGrowthType switch
        {
            AutoGrowthType.ByExtent => fileStream.Length + this.AutoGrowthAmount * (1L << 16),
            AutoGrowthType.ByPercent => (this.AutoGrowthAmount + 100L) * fileStream.Length / 100,
            _ => throw new CellaException("Invalid AutoGrowthType")
        };
        if (this.MaximumSize > 0)
            size = Math.Min(size, this.MaximumSize * (1L << 16));
        fileStream.SetLength(size);
    }
}