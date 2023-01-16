namespace Cella.Core;

public interface IPage
{
    bool IsDirty { get; }
    FullPageId FullPageId { get; }
    Task FlushAsync();
}