namespace Cella.Core;

public class Instance
{
    private readonly List<IDatabase> databases = new();
    private readonly IDatabaseMill databaseMill;

    public Instance(IDatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    public IDatabase CreateDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions);
        this.databases.Add(database);
        return database;
    }
}
