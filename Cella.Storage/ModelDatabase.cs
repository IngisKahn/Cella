namespace Cella.Storage;

using Files;

public class ModelDatabase : Database
{
    public const string ModelDbName = "model";
    public ModelDatabase(FileMill fileMill) : base(fileMill, new(ModelDatabase.ModelDbName))
    {
    }

    public async Task CreateAsync()
    {
        throw new NotImplementedException();
    }
}