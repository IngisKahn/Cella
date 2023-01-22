namespace Cella.Storage.DataSpaces;

using Files;

public class PrimaryFileGroup : FileGroup
{
    public ManagedFile PrimaryFile { get; }

    public PrimaryFileGroup(Database database, FileMill fileMill, IEnumerable<FileOptions>? fileOptions)
        : base(database, fileMill, "primary", 1)
    {
        var fileOptionsEnumerable = fileOptions as FileOptions[] ?? (fileOptions ?? Array.Empty<FileOptions>()).ToArray();
        if (!fileOptionsEnumerable.Any())
        {
            this.PrimaryFile = this.FileMill.CreateManaged(this, new( database.Name + " Primary File", database.Name + ".mdf") { Type = DatabaseFileType.Pages });
            this.DataFiles.Add(this.PrimaryFile);
        }
        else
        {
            var dataFiles = fileOptionsEnumerable.Select(o => this.FileMill.Create(this, o)).ToArray();
            this.PrimaryFile = (ManagedFile)dataFiles[0];
            this.DataFiles.AddRange(dataFiles);
        }
    }

    public PrimaryFileGroup(Database database, FileMill fileMill, ManagedFile primaryFile)
        : base(database, fileMill, "primary", 1)
    {
        if (primaryFile.Type != DatabaseFileType.Pages)
            throw new ArgumentException("Primary data file must contain page data");
        this.PrimaryFile = primaryFile;
    }
}