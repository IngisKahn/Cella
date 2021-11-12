using System.Collections.ObjectModel;

namespace Cella.Core;

public class Database
{
    public Collection<DatabaseObject> Objects { get; set; }

    public List<DatabaseFile> LogFiles { get; } = new();
    public PrimaryFileGroup PrimaryFileGroup { get; set; }
    public List<FileGroup> FileGroups { get; } = new();
}

public class FileGroup
{
    public List<DatabaseFile> DataFiles { get; } = new();

}

public class PrimaryFileGroup : FileGroup
{
    public DatabaseFile PrimaryFile { get; set; }

}

public class DatabaseFile
{

}