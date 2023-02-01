namespace Cella.Storage;

public record ServerOptions(string Name, string Location, int Port)
{
    public DatabaseOptions? ModelDatabaseOptions { get; init; }
    public DatabaseOptions? MasterDatabaseOptions { get; init; }
}