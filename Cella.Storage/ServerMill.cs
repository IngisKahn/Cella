namespace Cella.Storage;

public class ServerMill 
{
    private readonly DatabaseMill databaseMill;

    public ServerMill(DatabaseMill databaseMill)
    {
        ArgumentNullException.ThrowIfNull(databaseMill);
        this.databaseMill = databaseMill;
    }

    // requires role sysadmin || server access CONTROL or ALTER || permission CREATE DATABASE
    public Server CreateNew(string name, string location) => new(this.databaseMill, name, location, Guid.NewGuid());

    public Server Load(string masterDbPath) => new(this.databaseMill, masterDbPath);
}