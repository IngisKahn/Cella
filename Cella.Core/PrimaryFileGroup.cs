namespace Cella.Core;

public class PrimaryFileGroup : FileGroup
{
    public ManagedFile PrimaryFile { get; }

    public PrimaryFileGroup(ManagedFile primaryFile)
        : base("primary")
    {
        if (primaryFile.Type != DatabaseFileType.Pages)
            throw new ArgumentException("Primary data file must contain page data");
        this.PrimaryFile = primaryFile;
    }
}