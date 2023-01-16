namespace Cella.Storage.Files;

public enum DatabaseFileState
{
    Online,
    Restoring,
    Recovering,
    RecoveryPending,
    Suspect,
    Offline,
    Defunct
}