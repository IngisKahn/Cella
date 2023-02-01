namespace Cella.Storage.Files;

public record FileOptions(string Name, string PhysicalName, DatabaseFileType Type = DatabaseFileType.Pages);

public record ManagedFileOptions(string Name, string PhysicalName, DatabaseFileType Type = DatabaseFileType.Pages) 
    : FileOptions(Name, PhysicalName, Type)
{
    public int Extents { get; init; } = 48;
    public int MaxExtents { get; init; } = -1;
    public AutoGrowthType AutoGrowthType { get; init; } = AutoGrowthType.ByExtent;
    public uint AutoGrowthAmount { get; init; } = 16;
}
