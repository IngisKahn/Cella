namespace Cella.Storage;

using System.Globalization;
using Core;
using Objects;

public interface IDatabase
{
    public IEnumerable<DataObject> DataObjects { get; }
    public void Add(DataObject dataObject);

    /// <summary>
    /// Name of database, unique within an instance
    /// </summary>
    string Name { get; set; }
    /// <summary>
    /// ID of the database, unique within an instance
    /// </summary>
    DatabaseId DatabaseId { get; }
    /// <summary>
    /// Unique id 
    /// </summary>
    Guid Id { get; }
    /// <summary>
    /// Non-NULL = ID of the source database of this database snapshot.
    /// NULL = Not a database snapshot.
    /// </summary>
    int? SourceId { get; }
    /// <summary>
    /// Date the database was created or renamed. For tempdb, this value changes every time the server restarts.
    /// </summary>
    DateTime CreatedOn { get; }
    CultureInfo CollationCulture { get; set; }
    CompareOptions CollationOptions { get; set; }
    DatabaseUserAccess UserAccess { get; set; }
    bool IsReadOnly { get; set; }
    bool IsAutoClose { get; set; }
    DatabaseStatus Status { get; }
    /// <summary>
    /// Database is read-only for restore log.
    /// </summary>
    bool IsInStandby { get; }
    /// <summary>
    /// Database shut down cleanly; no recovery required on startup
    /// </summary>
    bool IsCleanlyShutdown { get; }
    bool IsSupplementalLoggingEnabled { get; }
    SnapshotIsolationState SnapshotIsolationState { get; }
    /// <summary>
    /// Read operations under the read-committed isolation level are based on snapshot scans and do not acquire locks.
    /// </summary>
    bool IsReadCommittedSnapshotOn { get; }
    RecoveryMode RecoveryMode { get; }
    PageVerifyMode PageVerifyMode { get; }
    bool IsAutoCreateStatsOn { get; }
    bool IsAutoCreateStatsIncrementalOn { get; }

    CultureInfo DefaultCultureInfo { get; }
    CultureInfo DefaultFullTextCultureInfo { get; }
    bool IsTransformNoiseWordsOn { get; }
    int TargetRecoveryTimeInSeconds { get; }
    DelayedDurability DelayedDurability { get; }
    bool IsMemoryOptimizedElevateToSnapshotOn { get; }
    bool IsMixedPageAllocationOn { get; }
    bool IsTemporalHistoryRetentionEnabled { get; }
    bool IsAcceleratedDatabaseRecoveryOn { get; }
}
