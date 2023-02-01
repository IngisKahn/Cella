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
    public Task<Server> CreateAsync(ServerOptions serverOptions) => Server.CreateAsync(this.databaseMill, serverOptions);

    public Task<Server> LoadAsync(string masterDbPath) => Server.LoadAsync(this.databaseMill, masterDbPath);
}