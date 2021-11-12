namespace Cella.Core;

public class PrimaryFileGroup : FileGroup
{
    public DatabaseFile PrimaryFile { get; set; }

    public PrimaryFileGroup(DatabaseFile primaryFile, IEnumerable<DatabaseFile>? files = null) 
        : base("primary", files?.Prepend(primaryFile) ?? new[] { primaryFile }) =>
        this.PrimaryFile = primaryFile;
}