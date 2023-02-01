namespace Cella.Storage;

using Files;

public class MasterDatabase : Database
{
    public const string MasterDbName = "master";
    public ModelDatabase Model { get; }

    public MasterDatabase(FileMill fileMill, Server server, ModelDatabase model) : base(fileMill, new(MasterDatabase.MasterDbName))
    {
        this.Model = model;
    }
    public ServerOptions ServerOptions { get; set; }
    public Guid ServerGuid { get; } = Guid.NewGuid();
    public IEnumerable<Database> Databases { get; } = new List<Database>();

}