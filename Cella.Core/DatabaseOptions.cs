namespace Cella.Core;

public record DatabaseOptions(string Name)
{
    public IEnumerable<FileOptions>? PrimaryFiles { get; init; }
    public IEnumerable<FileGroupOptions>? FileGroups { get; init; }
    public IEnumerable<FileOptions>? LogFiles { get; init; }
}
