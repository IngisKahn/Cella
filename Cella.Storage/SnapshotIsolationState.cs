namespace Cella.Storage;

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