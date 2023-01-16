namespace Cella.Storage.Objects;

using System.ComponentModel.DataAnnotations;

public enum ObjectType
{
    Table,
    InternalTable,
    SystemBaseTable,
    ExternalTable,
    View,
    PrimaryKey,
    UniqueConstraint,
    Check,
    Default,
    ForeignKey,
    AggregateFunction,
    ScalarFunction,
    TableFunction,
    StoredProcedure,
    PlanGuide,
    Synonym,
    Sequence,
    Edge,
    Node,
    ServiceQueue,
    SchemaTrigger,
    TableType
}

public abstract class DataObject
{
    //public void Store() {}
    //public void Manipulate() {}
    [Key]
    public int Id { get; }
    public string Name { get; }
    public Schema Schema { get; }
    public DataObject? Parent { get; }

    public ObjectType Type { get; } // OO
    public DateTime CreatedOn { get; }
    public DateTime ModifiedOn { get; }
    public bool IsInternal { get; }
    public bool IsPublished { get; }
    public bool IsSchemaPublished { get; }
    public void CopyTo(IDatabase database) => throw new NotImplementedException();
}

public enum IndexType
{
    Heap,
    Clustered,
    NonClustered,
    Object,
    Spatial,
    ClusteredColumnStore,
    NonClusteredColumnStore,
    NonClusteredHash
}

public class Index : DataObject
{

}

public class Table : DataObject
{

}