namespace Cella.Storage.DataSpaces;

using Files;

public class PrimaryFileGroup : FileGroup
{
    public ManagedFile PrimaryFile { get; }

    public PrimaryFileGroup(FileMill fileMill, IEnumerable<FileOptions> fileOptions)
        : base(fileMill, "primary", 1)
    {
        var fileOptionsEnumerable = fileOptions as FileOptions[] ?? fileOptions.ToArray();
        if (fileOptionsEnumerable.Length == 0)
            throw new ArgumentException("array cannot be empty", nameof(fileOptions));
        var dataFiles = fileOptionsEnumerable.Select(o => this.FileMill.Create(new(0), o)).ToArray();
        this.PrimaryFile = (ManagedFile)dataFiles[0];
        this.DataFiles.AddRange(dataFiles);
    }
    public PrimaryFileGroup(FileMill fileMill, string databaseName)
        : base(fileMill, "primary", 1)
    {
        this.PrimaryFile = this.FileMill.CreateManaged(new(), new(databaseName + " Primary File", databaseName + ".mdf") { Type = DatabaseFileType.Pages });
        this.DataFiles.Add(this.PrimaryFile);
    }

    public PrimaryFileGroup(Database database, FileMill fileMill, ManagedFile primaryFile)
        : base(fileMill, "primary", 1)
    {
        if (primaryFile.Type != DatabaseFileType.Pages)
            throw new ArgumentException("Primary data file must contain page data");
        this.PrimaryFile = primaryFile;
    }
}