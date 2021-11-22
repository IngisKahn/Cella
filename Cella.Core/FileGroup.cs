namespace Cella.Core;

public class FileGroup
{
    public List<DatabaseFile> DataFiles { get; }
    public string Name { get; }
    public bool AutoGrowAllFiles { get; set; }
    public FileGroup(string name) => this.Name = name;
}