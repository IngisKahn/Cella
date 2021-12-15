namespace Cella.Core;

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
    public int Id { get; }
    public string Name { get; }
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

public class Index
{
    public int Id { get; }
    public string Name { get; }
    public IndexType Type { get; }

}