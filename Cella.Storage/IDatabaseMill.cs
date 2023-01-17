namespace Cella.Storage;

public interface IDatabaseMill
{
    IDatabase Create(DatabaseOptions databaseOptions);
    IDatabase Create(DatabaseOptions databaseOptions, IDatabase model);

    IDatabase Load(string primaryDbPath);
    IMasterDatabase LoadMaster(string masterDbPath);
}