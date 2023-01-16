namespace Cella.Storage.Files;

using System.Threading.Tasks;

public interface IManagedFile : IDatabaseFile
{
    uint AutoGrowthAmount { get; init; }
    AutoGrowthType AutoGrowthType { get; init; }
    int InitialSize { get; init; }
    int MaximumSize { get; init; }
    int Size { get; set; }

    ValueTask DisposeAsync();
    void Grow();
}