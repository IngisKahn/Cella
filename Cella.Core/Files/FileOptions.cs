namespace Cella.Core;

public record FileOptions(string Name, string PhysicalName)
{
    public int Extents { get; init; } = 48;
    public int MaxExtents { get; init; } = -1;
    public AutoGrowthType AutoGrowthType { get; init; } = AutoGrowthType.ByExtent;
    public uint AutoGrowthAmount { get; init; } = 16;

    public DatabaseFileType Type { get; init; } = DatabaseFileType.Fixed;
}
