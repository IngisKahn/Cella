namespace Cella.Storage;

using System;
using System.Runtime.CompilerServices;
using Core;

public class Server
{
    private readonly Dictionary<string, Database> databases = [];
    private readonly DatabaseMill databaseMill;

    public ServerOptions Options { get; private init; } = null!;
    public Guid Guid { get; private init; } 

    public required ModelDatabase Model { private get; init; }
    public required MasterDatabase Master { private get; init; }

    private Server(DatabaseMill databaseMill, ServerOptions serverOptions)
    {
        this.databaseMill = databaseMill;
        this.Options = serverOptions;
        this.Guid = Guid.NewGuid();
    }

    private Server(DatabaseMill databaseMill, string masterDbPath, int port = 1319)
    {
        this.databaseMill = databaseMill;
        this.Options = new(Path.GetFileNameWithoutExtension(masterDbPath), Path.GetDirectoryName(masterDbPath) ?? throw new ArgumentException("Invalid Path", nameof(masterDbPath)), port);
    }

    internal static async Task<Server> CreateAsync(DatabaseMill databaseMill, ServerOptions serverOptions)
    {
        var model = await databaseMill.CreateModelAsync(serverOptions.Location, serverOptions.ModelDatabaseOptions);
        Server server = new(databaseMill, serverOptions)
        {
            Model = model,
            Master = await databaseMill.CreateMasterAsync(model, serverOptions.Location, serverOptions.MasterDatabaseOptions)
        };
        return server;
    }

    public static async Task<Server> LoadAsync(DatabaseMill databaseMill, string masterDbPath)
    {
        var master = await databaseMill.LoadMasterAsync(masterDbPath);

        // get server options
        var options = master.ServerOptions;

        // load model
        var model = master.Model;
        // load online dbs


        Server server = new(databaseMill, masterDbPath)
        {
            Master = master,
            Model = new(null),

            Options = new("","",0)
        };
        return server;
    }

    public Task AttachAsync(Database database) => throw new NotImplementedException();
    public Task DetachAsync(DatabaseId databaseId) => throw new NotImplementedException();
}