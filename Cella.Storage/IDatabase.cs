namespace Cella.Storage;

using System.Globalization;
using Core;
using Objects;

public enum DatabaseUserAccess
{
    Multi,
    Single,
    Restricted
}

public enum DatabaseStatus
{
    Online,
    Restoring,
    Recovering,
    RecoveryPending,
    Suspect,
    Emergency,
    Offline
}

public enum SnapshotIsolationState
{
    /// <summary>
    /// Snapshot isolation is disallowed.
    /// </summary>
    Off,
    /// <summary>
    /// Snapshot isolation is allowed.
    /// </summary>
    On,
    /// <summary>
    /// Snapshot isolation state is in transition to OFF state. All transactions have their modifications versioned. Cannot start new transactions using snapshot isolation. The database remains in the transition to OFF state until all transactions that were active when ALTER DATABASE was run can be completed.
    /// </summary>
    TurningOff,
    /// <summary>
    /// Snapshot isolation state is in transition to ON state. New transactions have their modifications versioned. Transactions cannot use snapshot isolation until the snapshot isolation state becomes 1 (ON). The database remains in the transition to ON state until all update transactions that were active when ALTER DATABASE was run can be completed.
    /// </summary>
    TurningOn
}

public enum DelayedDurability
{
    Disabled,
    Allowed,
    Forced
}

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
