namespace Cella.Storage.Files;

using DataSpaces;

public class FileMill
{
    public DatabaseFile Create(FileGroup fileGroup, FileOptions options) => options.Type switch
    {
        DatabaseFileType.FileStream => throw new NotImplementedException(),
        _ => new ManagedFile(fileGroup, new(), options.Name, options.PhysicalName, options.Type)
    };

    public ManagedFile CreateManaged(FileGroup fileGroup, FileOptions options) => new ManagedFile(fileGroup, new(), options.Name, options.PhysicalName, options.Type);
}