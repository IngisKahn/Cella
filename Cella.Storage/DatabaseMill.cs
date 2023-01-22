namespace Cella.Storage;

using Files;

public class DatabaseMill
{
    private readonly FileMill fileMill;
    public DatabaseMill(FileMill fileMill)
    {
        ArgumentNullException.ThrowIfNull(fileMill);
        this.fileMill = fileMill;
    }
    public Database Create(DatabaseOptions databaseOptions) => new Database(fileMill, databaseOptions);

    public Database Create(DatabaseOptions databaseOptions, Database model)
    {
        var database = new Database(fileMill, databaseOptions.ApplyDefaults(model.Options)) {};
        foreach (var dataObject in model.DataObjects)
            database.Add(dataObject);
        return database;
    }

    public Database Load(string primaryDbPath) => throw new NotImplementedException();

    public MasterDatabase LoadMaster(string masterDbPath) => throw new NotImplementedException();
}
