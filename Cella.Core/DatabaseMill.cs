namespace Cella.Core;

public class DatabaseMill : IDatabaseMill
{
    private readonly IFileMill fileMill;
    public DatabaseMill(IFileMill fileMill)
    {
        ArgumentNullException.ThrowIfNull(fileMill);
        this.fileMill = fileMill;
    }
    public IDatabase Create(DatabaseOptions databaseOptions) => new Database(fileMill, databaseOptions);
}
