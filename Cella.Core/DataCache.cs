using System.Collections.Concurrent;
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
    private readonly ConcurrentQueue<Slot> dirtyPages = new();
    private readonly Task flusher;
    private readonly ManualResetEventSlim dirtyAvailable = new();

    public DataCache(BufferPool bufferPool, Options options)
    {
        this.bufferPool = bufferPool;
        this.options = options;
        this.lazyWriter = Task.Run(this.LazyWriterWorkerAsync);
        this.flusher = Task.Run(this.FlusherAsync);
    }

    public async ValueTask DisposeAsync()
    {
        this.shutdown = true;
        this.dirtyAvailable.Set();
        await this.lazyWriter;
        foreach (var freeBuffer in freeBuffers)
            freeBuffer.Dispose();
        foreach (var slot in allocated)
            slot.Buffer.Dispose();
        await this.flusher;
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
                var currentPage = currentSlot.Page;
                lock (currentSlot)
                {
                    if (now - currentSlot.LastAccess <= this.options.CorrelatedReferencePeriod)
                        continue;
                    currentSlot.Invalid = true;
                    if (currentPage.IsDirty)
                    {
                        currentSlot.WriteLock = new();
                        Monitor.Enter(currentSlot.WriteLock);
                        this.dirtyPages.Enqueue(currentSlot);
                        this.dirtyAvailable.Set();
                    }
                    else
                    {
                        cache.TryRemove(currentPage.FullPageId, out _);
                        lock (allocated)
                            allocated.Remove(currentNode);
                        freeBuffers.Enqueue(currentSlot.Buffer);
                    }
                }
            }

            await Task.Delay(this.options.LazyWriterInterval);
        } while (!shutdown);
    }

    private async Task FlusherAsync()
    {
        for (; ; )
        {
            this.dirtyAvailable.Wait();
            this.dirtyAvailable.Reset();
            if (this.shutdown)
                return;
            while (this.dirtyPages.TryDequeue(out var slot))
            {
                await slot.Page.FlushAsync();
                Monitor.Exit(slot.WriteLock!);
            }
        }
    }

    public async ValueTask<Page> GetPageAsync(FullPageId pageId, Func<Page> loader)
    {
        var ticks = Environment.TickCount64;
        if (!this.cache.TryGetValue(pageId, out var slot))
        {
            BufferPool.Buffer buffer;
            while (!freeBuffers.TryDequeue(out buffer))
                await Task.Delay(this.options.LazyWriterInterval);
            slot = new(buffer, loader()) { UncorrelatedAccess1 = ticks };
            lock (this.allocated)
                this.allocated.AddLast(slot);
            this.cache[pageId] = slot;
        }
        else if (ticks - slot.LastAccess > this.options.CorrelatedReferencePeriod)
        {
            slot.UncorrelatedAccess2 = slot.UncorrelatedAccess1 + (slot.LastAccess - slot.UncorrelatedAccess1);
            slot.UncorrelatedAccess1 = ticks;
        }

        slot.LastAccess = ticks;
        return slot.Page;
    }

    private record Slot(BufferPool.Buffer Buffer, Page Page)
    {
        public long LastAccess { get; set; }
        public long UncorrelatedAccess1 { get; set; }
        public long UncorrelatedAccess2 { get; set; }
        public bool Invalid { get; set; }
        public object? WriteLock { get; set; }
    }
}