namespace Cella.Core;

public readonly record struct FullPageId(DatabaseId DatabaseId, FileId FileId, PageId PageId);

public readonly record struct LocalPageId(FileId FileId, PageId PageId);

public readonly record struct DatabaseId(ushort Value);
public readonly record struct FileId(ushort Value);

public readonly record struct PageId(uint Value)
{
    public PageId Next => new(Value + 1);
    public PageId Previous => new(Value - 1);
}