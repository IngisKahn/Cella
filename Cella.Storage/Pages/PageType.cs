namespace Cella.Storage.Pages;

public enum PageType : byte
{
    Data = 1,
    IntermediateIndex,
    MixedData,
    MixedTree,
    // _
    // _
    Sort = 7, 
    GlobalAllocationMap,
    SharedGlobalAllocationMap,
    IndexAllocationMap,
    PageFreeSpace,
    // _
    Boot = 13,
    Server,
    File,
    Changed,
    BulkChanged
}