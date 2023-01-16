namespace Cella.Core;

public readonly record struct PageId(uint Value)
{
    public PageId Next => new(Value + 1);
    public PageId Previous => new(Value - 1);
}