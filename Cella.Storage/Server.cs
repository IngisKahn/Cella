namespace Cella.Storage;

public class Server
{
    private readonly Dictionary<string, IDatabase> databases = new();
    private readonly IDatabaseMill databaseMill;

    public string Name { get; }
    public string Location { get; }
    public Guid Id { get; }

    public Server(IDatabaseMill databaseMill, string name, string location, Guid id)
    {
        this.databaseMill = databaseMill;
        this.Name = name;
        this.Location = location;
        this.Id = id;
    }

    public IDatabase CreateEmptyDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions);
        this.databases.Add(database.Name, database);
        return database;
    }

    public IDatabase CreateDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions, this.databases["model"]);
        this.databases.Add(database.Name, database);
        // add to master
        return database;
    }
}

public class ServerMill 
{
    private readonly IDatabaseMill databaseMill;

    public ServerMill(IDatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    public Server CreateNew(string name, string location)
    {
        Server server = new(this.databaseMill, name, location, Guid.NewGuid());
        // create model
        var model = server.CreateEmptyDatabase(new("model"));
        // populate model
        // copy to master
        var master = server.CreateDatabase(new("master"));
        // populate master

        return server;
    }

    // OpenExisting(path to master)
}
