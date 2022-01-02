using System.Collections.ObjectModel;

namespace Cella.Core;

using System.Globalization;
using System.Runtime.CompilerServices;
using DataSpaces;
using Files;
using Objects;

public class Database : IDatabase
{
    private readonly IFileMill fileMill;
    public List<DataObject> Objects { get; set; } = new();
    // Indexes
    // Columns
    // IdentityColumns
    // IndexColumns
    // DefaultConstraints
    // ComputedColumns
    // Partitions
    // AllocationUnits
    // FileGroups
    // Files
    // NodeStores
    // EdgeStores

    // Files
    // FileGroups
    // Schemas

    public FileGroup LogFiles { get; }
    public PrimaryFileGroup PrimaryFileGroup { get; }
    public FileGroup[] FileGroups { get; }
    public IEnumerable<DataObject> DataObjects => this.Objects;
    public void Add(DataObject dataObject) => dataObject.CopyTo(this);

    public string Name { get; set; }
    public int Id { get; }
    public int? SourceId { get; }
    public DateTime CreatedOn { get; }
    public CultureInfo CollationCulture { get; set; }
    public CompareOptions CollationOptions { get; set; }
    public DatabaseUserAccess UserAccess { get; set; }
    public DatabaseStatus Status { get; } = DatabaseStatus.Offline;
    public bool IsInStandby { get; }
    public bool IsCleanlyShutdown { get; }
    public bool IsSupplementalLoggingEnabled { get; }
    public SnapshotIsolationState SnapshotIsolationState { get; }
    public bool IsReadCommittedSnapshotOn { get; }
    public RecoveryMode RecoveryMode { get; }
    public PageVerifyMode PageVerifyMode { get; }
    public bool IsAutoCreateStatsOn { get; }
    public bool IsAutoCreateStatsIncrementalOn { get; }
    public CultureInfo DefaultCultureInfo { get; }
    public CultureInfo DefaultFullTextCultureInfo { get; }
    public bool IsTransformNoiseWordsOn { get; }
    public int TargetRecoveryTimeInSeconds { get; }
    public DelayedDurability DelayedDurability { get; }
    public bool IsMemoryOptimizedElevateToSnapshotOn { get; }
    public bool IsMixedPageAllocationOn { get; }
    public bool IsTemporalHistoryRetentionEnabled { get; }
    public bool IsAcceleratedDatabaseRecoveryOn { get; }
    public bool IsReadOnly { get; set; }
    public bool IsAutoClose { get; set; }
    public FileGroup DefaultFileGroup { get; set; }

    public Database(IFileMill fileMill, DatabaseOptions databaseOptions)
    {
        this.fileMill = fileMill;
        this.Name = databaseOptions.Name;
        this.UserAccess = databaseOptions.UserAccess;
        this.PrimaryFileGroup = new(this, fileMill, databaseOptions.PrimaryFiles);

        var fileGroups =
            (databaseOptions.FileGroups ?? Array.Empty<FileGroupOptions>()).Select((fgo, i) =>
                new FileGroup(this, fileMill, fgo, i + 1));


        this.FileGroups = fileGroups.Prepend(this.PrimaryFileGroup).ToArray();

        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fileGroup => fileGroup.IsDefault) ?? this.FileGroups.First();

        this.LogFiles = new(this, fileMill, new("Log",databaseOptions.LogFiles != null && databaseOptions.LogFiles.Any() ? databaseOptions.LogFiles
            : new[]
            {
                new FileOptions(this.Name + " Log", this.Name + ".ldf")
                {
                    Extents = Math.Max(
                        this.FileGroups.Sum(fg => fg.DataFiles.OfType<IManagedFile>().Sum(df => df.InitialSize)), 8)
                }
            }), 0);
        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fg => fg.IsDefault) ?? this.PrimaryFileGroup;
    }
}