namespace Cella.Storage;

using Files;

public class MasterDatabase : Database
{
    public const string MasterDbName = "master";

    public MasterDatabase(FileMill fileMill, DatabaseOptions databaseOptions) : base(fileMill, databaseOptions)
    {
    }

    public string ServerName { get; } = "SERVER_NAME";
    public Guid ServerGuid { get; } = Guid.NewGuid();
    public IEnumerable<Database> Databases { get; } = new List<Database>();
}