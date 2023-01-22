namespace Cella.Storage.Objects;

using System.ComponentModel.DataAnnotations;

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
    public void CopyTo(Database database) => throw new NotImplementedException();
}