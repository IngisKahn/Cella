using Xunit;

namespace Cella.Core.Tests
{
    public class BufferPoolTests
    {
        [Fact]
        public void InitialSizeMatchesFreePages()
        {
            BufferPool bp = new(10);
            Assert.Equal(10, bp.FreePages);
        }
        [Fact]
        public void InitialSizeMatchesWorkingSetPages()
        {
            BufferPool bp = new(10);
            Assert.Equal(10, bp.WorkingSetPages);
        }

        [Fact]
        public void AllocatingTakesUpAPage()
        {
            BufferPool bp = new(5);
            var b = bp.Allocate();
            Assert.Equal(4, bp.FreePages);
            b.Dispose();
            Assert.Equal(5, bp.FreePages);
        }

        [Fact]
        public void MemoryIs8K()
        {
            BufferPool bp = new(5);
            using var b = bp.Allocate();
            Assert.Equal(1 << 13, b.Memory.Length);
        }

        [Fact]
        public void GrowWhenNeeded()
        {
            BufferPool bp = new(5);
            var bs = new BufferPool.Buffer[6];
            for (var i = 0; i < 5; i++)
                bs[i] = bp.Allocate();
            Assert.Equal(0, bp.FreePages);
            Assert.Equal(5, bp.WorkingSetPages);
            bs[5] = bp.Allocate();
            Assert.Equal(4, bp.FreePages);
            Assert.Equal(10, bp.WorkingSetPages);

            for (var i = 0; i < 6; i++)
                bs[i].Dispose();
        }
    }
}
