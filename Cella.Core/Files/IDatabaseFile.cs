namespace Cella.Core
{
    using System;

    public interface IDatabaseFile
    {
        FileGroup FileGroup { get; }
        Guid Guid { get; }
        ushort Id { get; }
        bool IsMediaReadOnly { get; init; }
        bool IsNameReserved { get; set; }
        bool IsReadOnly { get; set; }
        bool IsSparse { get; init; }
        string Name { get; }
        string PhysicalName { get; }
        DatabaseFileState State { get; set; }
        DatabaseFileType Type { get; }

        void Create();
        void Validate();
    }
}