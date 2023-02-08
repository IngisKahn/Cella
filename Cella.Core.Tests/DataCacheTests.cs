namespace Cella.Core.Tests
{
    using System.Threading.Tasks;
    using Storage.Pages;
    using Xunit;

    public class DataCacheTests
    {
        private class MockPage : Page
        {

        }

        private static readonly DataCache.Options options = 
            new(
            10000, 
            20000, 
            50, 
            100, 
            1000, 
            16, 
            2000);

        [Fact]
        public async Task GetsSamePage()
        {
            BufferPool bp = new(512);
            await using DataCache dc = new(bp, DataCacheTests.options);
            MockPage mp = new();
            FullPageId id = new();
            await dc.GetPageAsync(id, _ => mp);
            Assert.Equal(mp, await dc.GetPageAsync(id, _ => new MockPage()));
        }
    }
}
