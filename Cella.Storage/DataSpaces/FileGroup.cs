namespace Cella.Storage.DataSpaces;

using Files;

public class FileGroup : BaseFileGroup
{
    protected FileMill FileMill { get; }
    public List<DatabaseFile> DataFiles { get; } = [];
    public FileGroup(FileMill fileMill, FileGroupOptions options, int id) : base(id, options.Name, DataSpaceType.FileGroup)
    {
        this.FileMill = fileMill;
        this.IsDefault = options.IsDefault;
        this.DataFiles.AddRange(options.FileOptions.Select(fo => fileMill.Create(new(0), fo)));
    }
    protected FileGroup(FileMill fileMill, string name, int id) : base(id, name, DataSpaceType.FileGroup) => this.FileMill = fileMill;

    public void GrowRequest(ManagedFile file)
    {
        if (this.AutoGrowAllFiles)
            foreach (var managedFile in this.DataFiles.OfType<ManagedFile>())
                managedFile.Grow();
        else
            file.Grow();
    }
}