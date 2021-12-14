using System.Collections.ObjectModel;

namespace Cella.Core;

public class Database : IDatabase
{
    private readonly IFileMill fileMill;
    public Collection<DatabaseObject> Objects { get; set; } = new();

    public FileGroup LogFiles { get; }
    public PrimaryFileGroup PrimaryFileGroup { get; }
    public FileGroup[] FileGroups { get; }
    public string Name { get; set; }
    public DatabaseUserAccess UserAccess { get; set; }
    public DatabaseStatus Status { get; } = DatabaseStatus.Offline;
    public DatabaseMode Mode { get; set; }
    public bool IsAutoClose { get; set; }
    public FileGroup DefaultFileGroup { get; set; }

    public Database(IFileMill fileMill, DatabaseOptions databaseOptions)
    {
        this.fileMill = fileMill;
        this.Name = databaseOptions.Name;
        this.PrimaryFileGroup = new(this, fileMill, databaseOptions.PrimaryFiles);

        var fileGroups =
            (databaseOptions.FileGroups ?? Array.Empty<FileGroupOptions>()).Select(fgo =>
                new FileGroup(this, fileMill, fgo));


        this.FileGroups = fileGroups.Prepend(this.PrimaryFileGroup).ToArray();

        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fileGroup => fileGroup.IsDefault) ?? this.FileGroups.First();

        this.LogFiles = new(this, fileMill, new("Log",databaseOptions.LogFiles != null && databaseOptions.LogFiles.Any() ? databaseOptions.LogFiles
            : new[]
            {
                new FileOptions(this.Name + " Log", this.Name + ".ldf")
                {
                    Extents = Math.Max(
                        this.FileGroups.Sum(fg => fg.DataFiles.OfType<IManagedFile>().Sum(df => df.InitialSize)), 8)
                }
            }));
        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fg => fg.IsDefault) ?? this.PrimaryFileGroup;
    }
}