namespace Cella.Core.Pages;

public abstract class Page
{

}

public abstract class SlotPage : Page
{
}

public sealed class DataPage : SlotPage
{
}

public enum PageType : byte
{
    Data = 1,
    IntermediateIndex,
    MixedData,
    MixedTree,
    // _
    // _
    // Sort
    GlobalAllocationMap = 8,
    SharedGlobalAllocationMap,
    IndexAllocationMap,
    PageFreeSpace,
    // _
    Boot = 13
    // Server
    // File
    // Changed
    // BulkChanged
}