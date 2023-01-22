namespace Cella.Storage;

using System.Security.Principal;
using Core;

public class Server
{
    private readonly Dictionary<string, Database> databases = new();
    private readonly DatabaseMill databaseMill;

    public string Name { get; }
    public string Location { get; }
    public Guid Guid { get; }

    public Server(DatabaseMill databaseMill, string name, string location, Guid guid)
    {
        this.databaseMill = databaseMill;
        this.Name = name;
        this.Location = location;
        this.Guid = guid;
        // create model
        var model = this.CreateEmptyDatabase(new(Database.ModelDbName));
        this.databases.Add(Database.ModelDbName, model);
        // populate model
        // copy to master
        var master = this.CreateDatabase(new(MasterDatabase.MasterDbName));
        // populate master
    }

    public Server(DatabaseMill databaseMill, string masterDbPath)
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

    public Database CreateEmptyDatabase(DatabaseOptions databaseOptions)
    {
        var database = this.databaseMill.Create(databaseOptions);
        this.databases.Add(database.Name, database);
        return database;
    }

    public Database CreateDatabase(DatabaseOptions databaseOptions)
    {
        // requires role sysadmin || server access CONTROL or ALTER || permission CREATE DATABASE
        var database = this.databaseMill.Create(databaseOptions, this.databases[Database.ModelDbName]);
        this.databases.Add(database.Name, database);
        // add to master
        return database;
    }
}