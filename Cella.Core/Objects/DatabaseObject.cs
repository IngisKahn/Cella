namespace Cella.Core.Objects;

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

public class DatabaseObject
{
    //public void Store() {}
    //public void Manipulate() {}
    [Key]
    public int Id { get; }
    public string Name { get; }
    public Schema Schema { get; }
    public DatabaseObject? Parent { get; }

    public ObjectType Type { get; } // OO
    public DateTime CreatedOn { get; }
    public DateTime ModifiedOn { get; }
    public bool IsInternal { get; }
    public bool IsPublished { get; }
    public bool IsSchemaPublished { get; }
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

public class Index : DatabaseObject
{

}

public class Table : DatabaseObject
{

}