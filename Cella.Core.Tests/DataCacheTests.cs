using System.Threading.Tasks;
using Cella.Core.Pages;
using Xunit;

namespace Cella.Core.Tests
{
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
            await dc.GetPageAsync(id, () => mp);
            Assert.Equal(mp, await dc.GetPageAsync(id, () => new MockPage()));
        }
    }
}
