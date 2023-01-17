namespace Cella.Storage;

using Files;

public class DatabaseMill : IDatabaseMill
{
    private readonly IFileMill fileMill;
    public DatabaseMill(IFileMill fileMill)
    {
        ArgumentNullException.ThrowIfNull(fileMill);
        this.fileMill = fileMill;
    }
    public IDatabase Create(DatabaseOptions databaseOptions) => new Database(fileMill, databaseOptions);

    public IDatabase Create(DatabaseOptions databaseOptions, IDatabase model)
    {
        var database = new Database(fileMill, databaseOptions) {};
        foreach (var dataObject in model.DataObjects)
            database.Add(dataObject);
        return database;
    }

    public IDatabase Load(string primaryDbPath) => throw new NotImplementedException();

    public IMasterDatabase LoadMaster(string masterDbPath) => throw new NotImplementedException();
}
