namespace Cella.Storage;

using System.Globalization;
using Core;
using DataSpaces;
using Files;
using Objects;
using FileOptions = System.IO.FileOptions;

public class Database
{
    private readonly FileMill fileMill;
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
    public DatabaseId DatabaseId { get; }
    public Guid Id { get; }
    public int? SourceId { get; }
    public DateTime CreatedOn { get; }
    public CultureInfo CollationCulture { get; set; } = CultureInfo.InstalledUICulture;
    public CompareOptions CollationOptions { get; set; }
    public DatabaseUserAccess UserAccess { get; set; }
    public DatabaseStatus Status { get; } = DatabaseStatus.Offline;
    public bool IsInStandby { get; }
    public bool IsCleanlyShutdown { get; }
    public bool IsSupplementalLoggingEnabled { get; }
    public SnapshotIsolationState SnapshotIsolationState { get; }
    public bool IsAutoCreateStatsIncrementalOn { get; }
    public CultureInfo DefaultCultureInfo { get; } = CultureInfo.InstalledUICulture;
    public CultureInfo DefaultFullTextCultureInfo { get; } = CultureInfo.InstalledUICulture;
    public bool IsTransformNoiseWordsOn { get; }
    public DelayedDurability DelayedDurability { get; }
    public bool IsMemoryOptimizedElevateToSnapshotOn { get; }
    public bool IsMixedPageAllocationOn { get; }
    public bool IsTemporalHistoryRetentionEnabled { get; }
    public bool IsAcceleratedDatabaseRecoveryOn { get; }
    public bool IsReadOnly { get; set; }
    public FileGroup DefaultFileGroup { get; set; }
    public DatabaseOptions Options { get; }

    public Database(FileMill fileMill, DatabaseOptions databaseOptions)
    {
        this.fileMill = fileMill;
        this.Options = databaseOptions;
        this.Name = databaseOptions.Name;
        this.UserAccess = databaseOptions.UserAccess ?? DatabaseUserAccess.Multi;
        this.PrimaryFileGroup = databaseOptions.PrimaryFiles != null
            ? new(fileMill, databaseOptions.PrimaryFiles)
            : new(fileMill, this.Name);

        var fileGroups =
            (databaseOptions.FileGroups ?? Array.Empty<FileGroupOptions>()).Select((fgo, i) =>
                new FileGroup(fileMill, fgo, i + 1));


        this.FileGroups = fileGroups.Prepend(this.PrimaryFileGroup).ToArray();

        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fileGroup => fileGroup.IsDefault) ?? this.FileGroups.First();

        this.LogFiles = new(fileMill, new("Log",databaseOptions.LogFiles != null && databaseOptions.LogFiles.Any() ? databaseOptions.LogFiles
            : new[]
            {
                new Files.ManagedFileOptions(this.Name + " Log", this.Name + ".ldf")
                {
                    Extents = Math.Max(
                        this.FileGroups.Sum(fg => fg.DataFiles.OfType<ManagedFile>().Sum(df => df.InitialSize)), 8)
                }
            }), 0);
        this.DefaultFileGroup = this.FileGroups.FirstOrDefault(fg => fg.IsDefault) ?? this.PrimaryFileGroup;
    }
    public virtual async Task CreateAsync(ModelDatabase model)
    {
        throw new NotImplementedException();
    }

    public virtual async Task LoadAsync()
    {
        throw new NotImplementedException();
    }
}