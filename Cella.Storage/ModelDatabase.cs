namespace Cella.Storage;

using Files;
using Objects;

public class ModelDatabase(FileMill fileMill, DatabaseOptions databaseOptions) : Database(fileMill, new DatabaseOptions(ModelDatabase.ModelDbName).ApplyDefaults(databaseOptions))
{
    public const string ModelDbName = "model";

    public async Task CreateAsync()
    {
        // this is the only database that can be created without a model
        // we have to add all the tables and columns manually

        // table for each data object type
        // objects table
        // tables table
        // columns table
        // indexes table
        // index columns table
        // partitions table
        // allocation units table

        List<DataObject> dataObjects = [];
        dataObjects.Add(new Table("Objects"));
        dataObjects.Add(new Table("Tables"));


        throw new NotImplementedException();
    }
}