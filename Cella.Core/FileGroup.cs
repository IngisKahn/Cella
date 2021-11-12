namespace Cella.Core;

public class FileGroup
{
    public DatabaseFile[] DataFiles { get; }
    public string Name { get; }
    public bool AutoGrowAllFiles { get; set; }
    public FileGroup(string name, IEnumerable<DatabaseFile> files)
    {
        this.Name = name;
        this.DataFiles = files?.ToArray() ?? Array.Empty<DatabaseFile>();
    }
}