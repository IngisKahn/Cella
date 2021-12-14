namespace Cella.Core;

public class Instance
{
    private readonly List<IDatabase> databases = new();
    private readonly IDatabaseMill databaseMill;

    public string Name { get; }
    public string Location { get; }

    public Instance(IDatabaseMill databaseMill, string name, string location)
    {
        this.databaseMill = databaseMill;
        this.Name = name;
        this.Location = location;
    }

    public IDatabase CreateDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions);
        this.databases.Add(database);
        // add to master
        return database;
    }
}

public class InstanceLoader //name sucks
{
    private readonly IDatabaseMill databaseMill;

    public InstanceLoader(IDatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    public Instance CreateNew(string name, string location)
    {
        Instance instance = new(this.databaseMill, name, location);
        // create model
        // populate model
        // copy to temp
        // copy to master
        // populate master

        return instance;
    }

    // OpenExisting
}
