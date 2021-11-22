using System.Collections.ObjectModel;

namespace Cella.Core;

public class Database
{
    public Collection<DatabaseObject> Objects { get; set; } = new();

    public DatabaseFile[] LogFiles { get; }
    public PrimaryFileGroup PrimaryFileGroup { get; }
    public FileGroup[] FileGroups { get; }
    public string Name { get; set; }
    public FileGroup DefaultFileGroup { get; set; }

    public Database(string name, PrimaryFileGroup primaryFileGroup, IEnumerable<FileGroup> fileGroups, IEnumerable<DatabaseFile> logFiles)
    {
        this.Name = name;
        this.PrimaryFileGroup = primaryFileGroup;
        this.FileGroups = fileGroups.Prepend(primaryFileGroup).ToArray();
        this.LogFiles = logFiles.ToArray();
        this.DefaultFileGroup = primaryFileGroup;
    }
}