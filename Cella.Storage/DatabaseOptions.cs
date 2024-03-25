namespace Cella.Storage;

using DataSpaces;
using Files;
using FileOptions = Files.FileOptions;

public record DatabaseOptions(string Name)
{
    public DatabaseUserAccess? UserAccess { get; init; }// = DatabaseUserAccess.Multi;
    public bool? IsAutoClose { get; init; }
    public bool? IsAutoCreateStatistics { get; init; }// = true;
    public bool? IsAutoUpdateStatistics { get; init; }// = true;
    public RecoveryMode? Recovery { get; init; }
    public PageVerifyMode? PageVerify { get; init; }
    public int? TargetRecoverySeconds { get; init; }
    public bool? AllowSnapshotIsolation { get; init; }// = true;
    public bool? ReadUncommittedSnapshot { get; init; }
    public IEnumerable<FileOptions>? PrimaryFiles { get; init; }
    public IEnumerable<FileGroupOptions>? FileGroups { get; init; }
    public IEnumerable<FileOptions>? LogFiles { get; init; }

    public DatabaseOptions ApplyDefaults(DatabaseOptions defaults) =>
        new(this.Name)
        {
            UserAccess = this.UserAccess ?? defaults.UserAccess,
            IsAutoClose = this.IsAutoClose ?? defaults.IsAutoClose,
            IsAutoCreateStatistics = this.IsAutoCreateStatistics ?? defaults.IsAutoCreateStatistics,
            IsAutoUpdateStatistics = this.IsAutoUpdateStatistics ?? defaults.IsAutoUpdateStatistics,
            Recovery = this.Recovery ?? defaults.Recovery,
            PageVerify = this.PageVerify ?? defaults.PageVerify,
            TargetRecoverySeconds = this.TargetRecoverySeconds ?? defaults.TargetRecoverySeconds,
            AllowSnapshotIsolation = this.AllowSnapshotIsolation ?? defaults.AllowSnapshotIsolation,
            ReadUncommittedSnapshot = this.ReadUncommittedSnapshot ?? defaults.ReadUncommittedSnapshot,
            PrimaryFiles = this.PrimaryFiles ?? defaults.PrimaryFiles,
            FileGroups = this.FileGroups ?? defaults.FileGroups,
            LogFiles = this.LogFiles ?? defaults.LogFiles,
        };
    public static DatabaseOptions Defaults(string? name = null) =>
        new(name ?? string.Empty)
        {
            UserAccess = DatabaseUserAccess.Multi,
            IsAutoClose = false,
            IsAutoCreateStatistics = true,
            IsAutoUpdateStatistics = true,
            Recovery = RecoveryMode.Full,
            PageVerify = PageVerifyMode.None,
            TargetRecoverySeconds = 0,
            AllowSnapshotIsolation = true,
            ReadUncommittedSnapshot = false,
            PrimaryFiles = new[] { new ManagedFileOptions(name,  name + ".mdf") },
            LogFiles = new [] { new ManagedFileOptions(name + "_log", name + ".ldf", DatabaseFileType.Log) },
        };
}