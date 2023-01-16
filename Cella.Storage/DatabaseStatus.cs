namespace Cella.Storage;

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