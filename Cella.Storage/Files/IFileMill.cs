namespace Cella.Storage.Files;

using DataSpaces;

public interface IFileMill
{
    IDatabaseFile Create(FileGroup fileGroup, FileOptions options);

    IManagedFile CreateManaged(FileGroup fileGroup, FileOptions options);
}


