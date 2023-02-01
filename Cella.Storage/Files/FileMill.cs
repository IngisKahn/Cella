namespace Cella.Storage.Files;

using DataSpaces;

public class FileMill
{
    public DatabaseFile Create(FileGroup fileGroup, FileOptions options) => options.Type switch
    {
        DatabaseFileType.FileStream => throw new NotImplementedException(),
        _ => CreateManaged(fileGroup, options as ManagedFileOptions ?? new ManagedFileOptions(options.Name, options.PhysicalName))
    };

    public ManagedFile CreateManaged(FileGroup fileGroup, ManagedFileOptions options) => new(fileGroup, new(), options.Name, options.PhysicalName, options.Type);
}