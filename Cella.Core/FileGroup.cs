namespace Cella.Core;

public class FileGroup
{
    public List<DatabaseFile> DataFiles { get; } = new();
    public string Name { get; }
    public bool AutoGrowAllFiles { get; set; }
    public FileGroup(string name) => this.Name = name;
    public void GrowRequest(ManagedFile file)
    {
        if (this.AutoGrowAllFiles)
            foreach (var managedFile in this.DataFiles.OfType<ManagedFile>())
                managedFile.Grow();
        else
            file.Grow();
    }
}