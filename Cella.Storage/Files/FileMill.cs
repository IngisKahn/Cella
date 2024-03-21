namespace Cella.Storage.Files;

using Core;
using DataSpaces;

public class FileMill
{
    public DatabaseFile Create(FileId fileId, FileOptions options) => options.Type switch
    {
        DatabaseFileType.FileStream => throw new NotImplementedException(),
        _ => this.CreateManaged(fileId, options as ManagedFileOptions ?? new ManagedFileOptions(options.Name, options.PhysicalName))
    };

    public ManagedFile CreateManaged(FileId fileId, ManagedFileOptions options) => new(fileId, options.Name, options.PhysicalName, options.Type);
}