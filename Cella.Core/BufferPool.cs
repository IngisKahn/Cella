namespace Cella.Core;

public class BufferPool
{
    private readonly List<Memory<byte>> bufferBlocks = new();
    private readonly int initialSize;
    private readonly HashSet<int> freeBuffers = new();
    private int bufferCount;

    public BufferPool(int initialSize)
    {
        this.initialSize = initialSize;
        this.Grow();
    }

    private void Grow()
    {
        this.bufferBlocks.Add(new byte[this.initialSize << 13]);
        this.freeBuffers.UnionWith(Enumerable.Range(this.bufferCount, this.initialSize));
        this.bufferCount += this.initialSize;
    }

    public Buffer Allocate()
    {
        int index;
        lock (this.freeBuffers)
        {
            if (this.freeBuffers.Count == 0)
                this.Grow();
            index = this.freeBuffers.First();
            this.freeBuffers.Remove(index);
        }
        var (blockIndex, subIndex) = Math.DivRem(index, this.initialSize);
        return new(this, index, this.bufferBlocks[blockIndex][(subIndex << 13)..((subIndex + 1) << 13)]);
    }

    private void Free(int index)
    {
        var (blockIndex, subIndex) = Math.DivRem(index, this.initialSize);
        this.freeBuffers.Add(blockIndex * this.initialSize + subIndex);
    }

    public readonly struct Buffer : IDisposable
    {
        private readonly BufferPool daddy;
        private readonly int index;
        public Memory<byte> Memory { get; }

        public Buffer(BufferPool daddy, int index, Memory<byte> memory)
        {
            this.daddy = daddy;
            this.index = index;
            this.Memory = memory;
        }

        public void Dispose() => this.daddy.Free(this.index);
    }
}