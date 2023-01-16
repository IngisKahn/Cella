namespace Cella.Storage;

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