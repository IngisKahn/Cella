namespace Cella.Storage.DataSpaces;

using FileOptions = Files.FileOptions;

public record FileGroupOptions(string Name, IEnumerable<FileOptions> FileOptions)
{
    public bool IsFileStream { get; init; }
    public bool IsDefault { get; init; }
}
