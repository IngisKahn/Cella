namespace Cella.Core;

public enum DatabaseUserAccess
{
    Single,
    Restricted,
    Multi
}

public enum DatabaseStatus
{
    Offline,
    Online,
    Emergency,
    Restoring,
    Recovering,
    RecoveryPending,
    Suspect
}

public enum DatabaseMode
{
    ReadOnly,
    ReadWrite
}

public interface IDatabase
{
    string Name { get; }
    DatabaseUserAccess UserAccess { get; set; }
    DatabaseStatus Status { get; }
    DatabaseMode Mode { get; set; }
    bool IsAutoClose { get; set; }

}
