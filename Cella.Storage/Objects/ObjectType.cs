namespace Cella.Storage.Objects;

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