namespace Cella.Storage.Objects;

using Core;
using Files;

public class Table : DataObject
{
    // hardcoded silly tables for now
    public class DatabaseFileRow
    {
        public FileId FileId { get; init; }
        public Guid FileGuid { get; init; }
        public DatabaseFileType Type { get; init; }
        public int DataSpaceId { get; init; }
        public string Name { get; init; }
        public string PhysicalName { get; init; }
        public DatabaseFileState State { get; init; }
        public int Extents { get; init; }
        public int MaxExtents { get; init; }
        public int Growth { get; init; }
        public bool IsMediaReadOnly { get; init; }
        public bool IsReadOnly { get; init; }
        public bool IsSparse { get; init; }
        public bool IsPercentGrowth { get; init; }
        public bool IsNameReserved { get; init; }

        public DatabaseFileRow()
        {}

        public DatabaseFileRow(BinaryReader reader)
        {



        }

        public void Write(BinaryWriter writer)
        {

        }
    }

    public class TableRow
    {
        public int Id { get; init; }
        public int Name { get; init; }
    }
}