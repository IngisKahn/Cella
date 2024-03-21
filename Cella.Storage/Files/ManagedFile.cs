namespace Cella.Storage.Files;

using Core;
using DataSpaces;

public class ManagedFile : DatabaseFile, IAsyncDisposable
{
    private FileStream? fileStream;

    public ManagedFile(FileId id, string name, string physicalName, DatabaseFileType type)
        : base(id, name, physicalName, type)
    {
    }

    public ManagedFile(FileId id, string name, string physicalName, Guid guid, DatabaseFileType type)
        : base(id, name, physicalName, type, guid)
    {
    }

    /// <summary>
    /// Must be odd
    /// </summary>
    public int InitialSize { get; init; } = 49;
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
        this.fileStream = File.Create(this.PhysicalName);
        this.fileStream.SetLength(this.InitialSize);
    }

    public void Grow()
    {
        if (this.fileStream == null)
            throw new CellaException($"File {this.PhysicalName} for {this.Name} is not open");
        var size = this.AutoGrowthType switch
        {
            AutoGrowthType.ByExtent => this.fileStream.Length + this.AutoGrowthAmount * (1L << 16),
            AutoGrowthType.ByPercent => (this.AutoGrowthAmount + 100L) * this.fileStream.Length / 100,
            _ => throw new CellaException("Invalid AutoGrowthType")
        };
        if (this.MaximumSize > 0)
            size = Math.Min(size, this.MaximumSize * (1L << 16));
        this.fileStream.SetLength(size);
    }

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return (this.fileStream ?? throw new CellaException($"File {this.PhysicalName} for {this.Name} is not open"))
            .DisposeAsync();
    }
}