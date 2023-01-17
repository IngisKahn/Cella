namespace Cella.Storage;

using System.Security.Principal;
using Core;

public class Server
{
    private readonly Dictionary<string, IDatabase> databases = new();
    private readonly IDatabaseMill databaseMill;

    public string Name { get; }
    public string Location { get; }
    public Guid Guid { get; }

    public Server(IDatabaseMill databaseMill, string name, string location, Guid guid)
    {
        this.databaseMill = databaseMill;
        this.Name = name;
        this.Location = location;
        this.Guid = guid;
        // create model
        var model = this.CreateEmptyDatabase(new(Database.ModelDbName) {});
        this.databases.Add(Database.ModelDbName, model);
        // populate model
        // copy to master
        var master = this.CreateDatabase(new(MasterDatabase.MasterDbName));
        // populate master
    }

    public Server(IDatabaseMill databaseMill, string masterDbPath)
    {
        this.databaseMill = databaseMill;
        var location = Path.GetDirectoryName(masterDbPath) ?? throw new ArgumentException("Invalid Path", nameof(masterDbPath));
        this.Location = location;
        var master = this.databaseMill.LoadMaster(masterDbPath);
        this.Name = master.ServerName;
        this.Guid = master.ServerGuid;
        this.databases.Add(MasterDatabase.MasterDbName, master);
        foreach (var db in master.Databases)
            this.databases.Add(db.Name, db);
    }

    public IDatabase CreateEmptyDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions);
        this.databases.Add(database.Name, database);
        return database;
    }

    public IDatabase CreateDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions, this.databases[Database.ModelDbName]);
        this.databases.Add(database.Name, database);
        // add to master
        return database;
    }
}