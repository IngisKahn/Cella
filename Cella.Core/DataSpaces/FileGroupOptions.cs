namespace Cella.Core.DataSpaces;

public record FileGroupOptions(string Name, IEnumerable<FileOptions> FileOptions)
{
    public bool IsFileStream { get; init; }
    public bool IsDefault { get; init; }
}
