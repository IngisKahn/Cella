using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Cella.Core.Pages;

namespace Cella.Core;

public sealed class DataCache : IAsyncDisposable
{
    public readonly record struct Options(
        long CorrelatedReferencePeriod, 
        long MaxSecondaryCorrelationPeriod, 
        int MinimumFreeBuffers, 
        int GrowBufferAllocationCount,
        int LazyWriterInterval,
        int LazyWriterSlotBatchSize,
        int LazyWriterSleepInterval
        );

    private readonly BufferPool bufferPool;
    private readonly Options options;

    private readonly ConcurrentQueue<BufferPool.Buffer> freeBuffers = new();
    private readonly ConcurrentDictionary<FullPageId, Slot> cache = new();
    private readonly LinkedList<Slot> allocated = new();
    private LinkedListNode<Slot>? nextNode;
    private readonly Task lazyWriter;
    private bool shutdown;
    private bool generationFlag;

    public DataCache(BufferPool bufferPool, Options options)
    {
        this.bufferPool = bufferPool;
        this.options = options;
        this.lazyWriter = Task.Run(this.LazyWriterWorkerAsync);
    }

    public async ValueTask DisposeAsync()
    {
        this.shutdown = true;
        await this.lazyWriter;
        foreach (var freeBuffer in freeBuffers)
            freeBuffer.Dispose();
        foreach (var slot in allocated)
            slot.Buffer.Dispose();
    }

    private async Task LazyWriterWorkerAsync()
    {
        do
        {
            if (this.freeBuffers.Count < this.options.MinimumFreeBuffers)
                for (var i = 0; i < this.options.GrowBufferAllocationCount; i++)
                    this.freeBuffers.Enqueue(this.bufferPool.Allocate());

            this.nextNode ??= this.allocated.First;

            if (this.nextNode == null)
            {
                await Task.Delay(this.options.LazyWriterSleepInterval);
                continue;
            }

            var toCheck = Math.Min(this.options.LazyWriterSlotBatchSize, this.allocated.Count);
            var now = Environment.TickCount64;
            while (toCheck-- > 0)
            {
                var currentNode = this.nextNode!;
                this.nextNode = currentNode.Next ?? this.allocated.First;
                var currentSlot = currentNode.Value;
                if (now - currentSlot.LastAccess <= this.options.CorrelatedReferencePeriod
                    || currentSlot.UncorrelatedAccess2 < this.options.MaxSecondaryCorrelationPeriod)
                    continue;
                lock (currentSlot)
                {
                    if (now - currentSlot.LastAccess <= this.options.CorrelatedReferencePeriod)
                        continue;
                    currentSlot.Invalid = true;
                    if (currentSlot.Page.IsDirty)
                        this.FlushPage(currentNode, true);
                    else
                        DeallocateNode(currentNode);
                }
            }

            await Task.Delay(this.options.LazyWriterInterval);
        } while (!shutdown);
    }

    private void DeallocateNode(LinkedListNode<Slot> node)
    {
        this.cache.TryRemove(node.Value.Page.FullPageId, out _);
        lock (this.allocated)
            this.allocated.Remove(node);
        this.freeBuffers.Enqueue(node.Value.Buffer);
    }

    private void FlushPage(LinkedListNode<Slot> node, bool removeOnWrite)
    {
        var slot = node.Value;
        slot.WriteLock ??= new();
        Monitor.Enter(slot.WriteLock);
        slot.GenerationFlag = !slot.GenerationFlag;
        Task.Run(async () =>
        {
            await slot.Page.FlushAsync();
            Monitor.Exit(slot.WriteLock!);
            if (removeOnWrite)
                this.DeallocateNode(node);
        });
    }

    public async ValueTask<Page> GetPageAsync(FullPageId pageId, Func<Page> loader)
    {
        var ticks = Environment.TickCount64;
        if (!this.cache.TryGetValue(pageId, out var slot))
        {
            BufferPool.Buffer buffer;
            while (!freeBuffers.TryDequeue(out buffer)) // learn from this
                await Task.Delay(this.options.LazyWriterInterval);
            slot = new(buffer, loader(), ticks, this.generationFlag);
            lock (this.allocated)
                this.allocated.AddLast(slot);
            this.cache[pageId] = slot;
        }
        else 
            slot.UpdateAccess(ticks, ticks - slot.LastAccess <= this.options.CorrelatedReferencePeriod);

        return slot.Page;
    }

    // call every min if 10MB of log used
    public void CheckPoint()
    {
        var currentFlag = this.generationFlag;
        this.generationFlag = !currentFlag;
        lock (this.allocated)
            foreach (var slot in this.allocated)
                slot.GenerationFlag = currentFlag;

        LinkedListNode<Slot> node;
        for (;;)
        {
            var tryNode = this.allocated.First;
            if (tryNode == null)
            {
                if (this.allocated.Count == 0)
                    return; // nothing to do
                continue;
            }

            node = tryNode;
            var slot = node.Value;
            Monitor.Enter(slot);
            if (slot.Invalid)
            {
                Monitor.Exit(slot);
                continue;
            }
        }
    }

    private record struct Slot(BufferPool.Buffer Buffer, Page Page)
    {
        public bool GenerationFlag { get; set; }
        public long LastAccess { get; private set; }
        public long UncorrelatedAccess1 { get; set; }
        public long UncorrelatedAccess2 { get; private set; }
        public bool Invalid { get; set; }
        public object? WriteLock { get; set; }
        public Slot(BufferPool.Buffer buffer, Page page, long ticks, bool generationFlag) : this(buffer, page)
        {
            this.GenerationFlag = generationFlag;
            this.LastAccess = this.UncorrelatedAccess1 = ticks;
        }

        public void UpdateAccess(long ticks, bool isCorrelated)
        {
            this.LastAccess = ticks;
            if (isCorrelated)
                return;

            this.UncorrelatedAccess2 = this.UncorrelatedAccess1 + (this.LastAccess - this.UncorrelatedAccess1);
            this.UncorrelatedAccess1 = ticks;
        }
    }
}