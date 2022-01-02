namespace Cella.Core;

using DataSpaces;

public record DatabaseOptions(string Name)
{
    public DatabaseUserAccess UserAccess { get; init; } = DatabaseUserAccess.Multi;
    public bool IsAutoClose { get; init; }
    public bool IsAutoCreateStatistics { get; init; } = true;
    public bool IsAutoUpdateStatistics { get; set; } = true;
    public RecoveryMode Recovery { get; set; }
    public PageVerifyMode PageVerify { get; set; }
    public int TargetRecoverySeconds { get; set; }
    public bool AllowSnapshotIsolation { get; set; } = true;
    public bool ReadUncommittedSnapshot { get; set; }
    public IEnumerable<FileOptions>? PrimaryFiles { get; init; }
    public IEnumerable<FileGroupOptions>? FileGroups { get; init; }
    public IEnumerable<FileOptions>? LogFiles { get; init; }
}

public enum RecoveryMode
{
    Full,
    BulkLogged,
    Simple
}

public enum PageVerifyMode
{
    None,
    TornPageDetection,
    Checksum
}
