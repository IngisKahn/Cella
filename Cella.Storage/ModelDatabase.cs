namespace Cella.Storage;

using Files;

public class ModelDatabase(FileMill fileMill) : Database(fileMill, new(ModelDatabase.ModelDbName))
{
    public const string ModelDbName = "model";

    public async Task CreateAsync()
    {
        throw new NotImplementedException();
    }
}