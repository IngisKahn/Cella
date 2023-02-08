namespace Cella.Storage.Pages;

public enum PageType : byte
{
    Data = 1,
    IntermediateIndex,
    MixedData,
    MixedTree,
    // _
    WorkFile = 6,
    Sort, 
    GlobalAllocationMap,
    SharedGlobalAllocationMap,
    AllocationUnitMap,
    PageFreeSpace,
    // _
    Boot = 13,
    Server,
    File,
    Changed,
    BulkChanged
}