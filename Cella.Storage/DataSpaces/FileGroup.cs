namespace Cella.Storage.DataSpaces;

using Files;

public abstract class BaseFileGroup : DataSpace
{
    public Guid Guid { get; init; }
    public bool IsReadOnly { get; set; }
    public bool AutoGrowAllFiles { get; set; }

    protected BaseFileGroup(int id, string name, DataSpaceType type) : base(id, name, type) { }
}

public class FileGroup : BaseFileGroup
{
    protected IFileMill FileMill { get; }
    public List<IDatabaseFile> DataFiles { get; } = new();
    public IDatabase Database { get; }
    public FileGroup(IDatabase database, IFileMill fileMill, FileGroupOptions options, int id) : base(id, options.Name, DataSpaceType.FileGroup)
    {
        this.FileMill = fileMill;
        this.Database = database;
        this.IsDefault = options.IsDefault;
        this.DataFiles.AddRange(options.FileOptions.Select(fo => fileMill.Create(this, fo)));
    }
    protected FileGroup(IDatabase database, IFileMill fileMill, string name, int id) : base(id, name, DataSpaceType.FileGroup)
    {
        this.FileMill = fileMill;
        this.Database = database;
    }
    public void GrowRequest(ManagedFile file)
    {
        if (this.AutoGrowAllFiles)
            foreach (var managedFile in this.DataFiles.OfType<ManagedFile>())
                managedFile.Grow();
        else
            file.Grow();
    }
}