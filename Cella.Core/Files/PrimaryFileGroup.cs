namespace Cella.Core;

public class PrimaryFileGroup : FileGroup
{
    public IManagedFile PrimaryFile { get; }

    public PrimaryFileGroup(IDatabase database, IFileMill fileMill, IEnumerable<FileOptions>? fileOptions)
        : base(database, fileMill, "primary")
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
            this.PrimaryFile = (IManagedFile)dataFiles[0];
            this.DataFiles.AddRange(dataFiles);
        }
    }

    public PrimaryFileGroup(IDatabase database, IFileMill fileMill, IManagedFile primaryFile)
        : base(database, fileMill, "primary")
    {
        if (primaryFile.Type != DatabaseFileType.Pages)
            throw new ArgumentException("Primary data file must contain page data");
        this.PrimaryFile = primaryFile;
    }
}