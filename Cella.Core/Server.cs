namespace Cella.Core;

public class Server
{
    private readonly List<IDatabase> databases = new();
    private readonly IDatabaseMill databaseMill;

    public string Name { get; }
    public string Location { get; }

    public Server(IDatabaseMill databaseMill, string name, string location)
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

public class ServerLoader //name sucks
{
    private readonly IDatabaseMill databaseMill;

    public ServerLoader(IDatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    public Server CreateNew(string name, string location)
    {
        Server server = new(this.databaseMill, name, location);
        // create model
        var model = server.CreateDatabase(new("model"));
        // populate model
        // copy to master
        // populate master

        return server;
    }

    // OpenExisting(path to master)
}
