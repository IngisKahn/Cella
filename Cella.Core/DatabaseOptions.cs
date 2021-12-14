namespace Cella.Core;

public record DatabaseOptions(string Name)
{
    public DatabaseUserAccess UserAccess { get; init; } = DatabaseUserAccess.Multi;
    public IEnumerable<FileOptions>? PrimaryFiles { get; init; }
    public IEnumerable<FileGroupOptions>? FileGroups { get; init; }
    public IEnumerable<FileOptions>? LogFiles { get; init; }
}
