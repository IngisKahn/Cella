namespace Cella.Core;

public interface IDatabaseMill
{
    IDatabase Create(DatabaseOptions databaseOptions);
}