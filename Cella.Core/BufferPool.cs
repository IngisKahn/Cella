namespace Cella.Core;

public class BufferPool
{
    private readonly List<Memory<byte>> bufferBlocks = [];
    private readonly int initialSize;
    private readonly HashSet<int> freeBuffers = [];
    private int bufferCount;

    public int WorkingSetPages => this.bufferBlocks.Count * this.initialSize;
    public int FreePages => this.freeBuffers.Count;

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

    public readonly struct Buffer(BufferPool daddy, int index, Memory<byte> memory) : IDisposable
    {
        public Memory<byte> Memory { get; } = memory;

        public void Dispose() => daddy.Free(index);
    }
}