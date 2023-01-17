namespace Cella.Storage.Files
{
    using System;
    using Core;
    using DataSpaces;

    public interface IDatabaseFile
    {
        DataSpace DataSpace { get; }
        /// <summary>
        /// Globally unique identifier (GUID) for the file. 
        /// </summary>
        Guid Guid { get; }
        /// <summary>
        /// The file identification number (unique for each database). 
        /// </summary>
        FileId Id { get; }
        bool IsMediaReadOnly { get; init; }
        bool IsNameReserved { get; set; }
        bool IsReadOnly { get; set; }
        bool IsSparse { get; init; }
        string Name { get; }
        string PhysicalName { get; }
        DatabaseFileState State { get; set; }
        DatabaseFileType Type { get; }
        decimal CreateLsn { get; set; }
        decimal DropLsn { get; set; }
        decimal ReadOnlyLsn { get; set; }
        decimal ReadWriteLsn { get; set; }
        decimal DifferentialBaseLsn { get; set; }

        void Create();
        void Validate();
    }
}