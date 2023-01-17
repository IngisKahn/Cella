namespace Cella.Storage;

public class ServerMill 
{
    private readonly IDatabaseMill databaseMill;

    public ServerMill(IDatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    public Server CreateNew(string name, string location) => new(this.databaseMill, name, location, Guid.NewGuid());

    public Server Load(string masterDbPath) => new(this.databaseMill, masterDbPath);
}