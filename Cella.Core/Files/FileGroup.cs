namespace Cella.Core;

public class FileGroup
{
    protected IFileMill FileMill { get; }
    public List<IDatabaseFile> DataFiles { get; } = new();
    public string Name { get; }
    public IDatabase Database { get; }
    public bool AutoGrowAllFiles { get; set; }
    public bool IsDefault { get; set; }
    public bool IsFileStream { get; set; }
    public FileGroup(IDatabase database, IFileMill fileMill, string name)
    {
        this.FileMill = fileMill;
        this.Name = name;
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