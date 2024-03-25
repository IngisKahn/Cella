namespace Cella.Storage.Pages;

public enum PageType : byte
{
    Data = 1,
    IntermediateIndex,
    OverflowData,
    LargeObject,
    // _
    WorkFile = 6,
    Sort, 
    ExtentAllocationMap,
    SharedExtentAllocationMap,
    AllocationUnitMap,
    PageFreeSpace,
    // _
    Boot = 13,
    Server,
    File,
    Changed,
    BulkChanged
}