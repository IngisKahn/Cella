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
    // requires role sysadmin || server access CONTROL or ALTER || permission CREATE DATABASE
    public Task<ModelDatabase> CreateModelAsync(string location, DatabaseOptions databaseOptions) => throw new NotImplementedException();
    public Task<MasterDatabase> CreateMasterAsync(ModelDatabase model, string location, DatabaseOptions databaseOptions) => throw new NotImplementedException();

    public async Task<Database> Create(DatabaseOptions databaseOptions, ModelDatabase model)
    {
        var database = new Database(fileMill, databaseOptions.ApplyDefaults(model.Options));
        await database.CreateAsync(model);
        return database;
    }

    public Task<Database> LoadAsync(string primaryDbPath) => throw new NotImplementedException();

    public Task<MasterDatabase> LoadMasterAsync(string masterDbPath) => throw new NotImplementedException();
}
