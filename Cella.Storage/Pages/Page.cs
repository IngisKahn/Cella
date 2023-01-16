namespace Cella.Storage.Pages;

using Core;

public abstract class Page : IPage
{
    public FullPageId FullPageId { get; set; }
    public bool IsDirty { get; set; }

    public Task FlushAsync()
    {
        throw new NotImplementedException();
    }
}