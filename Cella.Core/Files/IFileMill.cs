namespace Cella.Core;

public interface IFileMill
{
    IDatabaseFile Create(FileGroup fileGroup, FileOptions options);

    IManagedFile CreateManaged(FileGroup fileGroup, FileOptions options);
}


