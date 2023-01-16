namespace Cella.Core;

using System.Collections.Concurrent;

public sealed class DataCache : IAsyncDisposable
{
    private readonly LinkedList<Slot> allocated = new();

    private readonly BufferPool bufferPool;
    private readonly ConcurrentDictionary<FullPageId, Slot> cache = new();

    private readonly ConcurrentQueue<BufferPool.Buffer> freeBuffers = new();
    private readonly Task lazyWriter;
    private readonly Options options;
    private bool generationFlag;
    private LinkedListNode<Slot>? nextWriterNode;
    private bool shutdown;

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
        foreach (var freeBuffer in this.freeBuffers)
            freeBuffer.Dispose();

        foreach (var slot in this.allocated)
            slot.Buffer.Dispose();
    }

    private void GrowBuffers()
    {
        for (var i = 0; i < this.options.GrowBufferAllocationCount; i++)
            this.freeBuffers.Enqueue(this.bufferPool.Allocate());
    }
    private static void SpinLock(Slot slot)
    {
        for (; Interlocked.CompareExchange(ref slot.WriteLock, 1, 0) == 1;) { }
    }
    private static bool TryLock(Slot slot) => Interlocked.CompareExchange(ref slot.WriteLock, 1, 0) == 0;

    private static void Unlock(Slot slot) => Interlocked.Decrement(ref slot.WriteLock);

    private async Task LazyWriterWorkerAsync()
    {
        do
        {
            // check if we are running low
            if (this.freeBuffers.Count < this.options.MinimumFreeBuffers)
                this.GrowBuffers();

            // if we didn't have any buffers allocated we need to start again
            this.nextWriterNode ??= this.allocated.First;

            // there are no buffers allocated, so take a nap
            if (this.nextWriterNode == null)
            {
                await Task.Delay(this.options.LazyWriterSleepInterval);
                continue;
            }

            var previousNode = this.nextWriterNode.Previous!;
            var currentNode = this.nextWriterNode!;
            var nextNode = this.nextWriterNode.Next!;

            // lock previous
            if (previousNode != currentNode) DataCache.SpinLock(previousNode!.Value);
            // lock current
            DataCache.SpinLock(currentNode.Value);

            var toCheck = Math.Min(this.options.LazyWriterSlotBatchSize, this.allocated.Count);
            var now = Environment.TickCount64;
            while (toCheck-- > 0) // let's try to free a few
            {
                if (nextNode != previousNode) // lock next node if different
                    DataCache.SpinLock(nextNode!.Value);

                var currentSlot = currentNode.Value;

                if (now - currentSlot.LastAccess > this.options.CorrelatedReferencePeriod &&
                    currentSlot.UncorrelatedAccess2 >= this.options.MaxSecondaryCorrelationPeriod)
                { // it needs to go
                    currentSlot.Invalid = true;
                    if (currentSlot.Page.IsDirty)
                        this.FlushPage(currentNode, true);
                    else
                        this.DeallocateNode(currentNode);
                    DataCache.Unlock(currentSlot); // he might have been grabbed by the allocator
                }
                else // move on
                {
                    previousNode = currentNode;

                    if (nextNode != previousNode)
                        DataCache.Unlock(previousNode.Value);

                    currentNode = nextNode;
                }

                this.nextWriterNode = nextNode = currentNode.Next;
            }

            DataCache.Unlock(currentNode.Value);
            if (currentNode != nextNode)
                DataCache.Unlock(nextNode!.Value);

            await Task.Delay(this.options.LazyWriterInterval);
        } while (!this.shutdown);
    }

    private void DeallocateNode(LinkedListNode<Slot> node)
    {
        this.allocated.Remove(node);
        this.cache.TryRemove(node.Value.Page.FullPageId, out _);

        this.freeBuffers.Enqueue(node.Value.Buffer);
    }

    private void FlushPage(LinkedListNode<Slot> node, bool removeOnWrite)
    {
        var slot = node.Value;
        if (removeOnWrite)
        {
            this.allocated.Remove(node);
            this.cache.TryRemove(slot.Page.FullPageId, out _);
        }

        slot.GenerationFlag = !slot.GenerationFlag;
        Task.Run(async () =>
        {
            await slot.Page.FlushAsync();
            if (removeOnWrite)
                this.freeBuffers.Enqueue(node.Value.Buffer);
        });
    }

    private static LinkedListNode<Slot>? TryLockValid(Func<LinkedListNode<Slot>?> getNode)
    {
        LinkedListNode<Slot>? node;
        for (node = getNode(); node != null; node = getNode())
        {
            DataCache.SpinLock(node.Value);
            if (!node.Value.Invalid)
                continue;

            DataCache.Unlock(node.Value);
        }

        return node;
    }

    public async ValueTask<IPage> GetPageAsync(FullPageId pageId, Func<IPage> loader)
    {
        var ticks = Environment.TickCount64;
        var gotIt = false;
        if (this.cache.TryGetValue(pageId, out var slot)) // do we have this page loaded?
        {
            DataCache.SpinLock(slot); // make sure it's real

            if (!slot.Invalid)
            {  // it's cool, mark it used and let it go
                slot.UpdateAccess(ticks, ticks - slot.LastAccess <= this.options.CorrelatedReferencePeriod);
                DataCache.Unlock(slot);
                gotIt = true;
            }
        }

        if (gotIt)
            return slot!.Page;

        // get a free buffer
        BufferPool.Buffer buffer;
        while (!this.freeBuffers.TryDequeue(out buffer)) // learn from this
            await Task.Delay(this.options.LazyWriterInterval); 

        slot = new(buffer, loader(), ticks, this.generationFlag) { WriteLock = 1 };



        var last = DataCache.TryLockValid(() => this.allocated.Last)?.Value;

        var first = last != null ? DataCache.TryLockValid(() => this.allocated.First)?.Value : null;

        this.allocated.AddLast(slot);

        if (last != null)
            DataCache.Unlock(last);
        DataCache.Unlock(slot);
        if (first != null)
            DataCache.Unlock(first);

        this.cache[pageId] = slot;

        return slot.Page;
    }

    // call every min if 10MB of log used
    public async Task CheckPoint()
    {
        var currentFlag = this.generationFlag;
        this.generationFlag = !currentFlag;

        var node = DataCache.TryLockValid(() => this.allocated.First);

        if (node != null)
        {
            var current = node;
            for (; ; )
            {
                current!.Value.GenerationFlag = currentFlag;
                var next = current.Next;
                var end = next == node;
                if (!end)
                    DataCache.SpinLock(next!.Value);
                DataCache.Unlock(current.Value);
                if (end)
                    break;
                current = next;
            }
        }
        var currentNode = DataCache.TryLockValid(() => this.allocated.First);
        if (currentNode == null)
            return; // nothing to do

        for (; ; )
        {
            if (currentNode.Value.GenerationFlag == currentFlag && currentNode.Value.Page.IsDirty)
            {
                currentNode.Value.GenerationFlag = !currentFlag;
                LinkedList<Slot> gatherList = new();
                gatherList.AddFirst(currentNode);

                var currentId = currentNode.Value.Page.FullPageId;

                while (this.cache.TryGetValue(currentId with { PageId = currentId.PageId.Previous}, out var previous) && DataCache.TryLock(previous))
                    if (!previous.Invalid && previous.GenerationFlag == currentFlag && previous.Page.IsDirty)
                    {
                        gatherList.AddFirst(previous);
                        previous.GenerationFlag = !currentFlag;
                        currentId = previous.Page.FullPageId;
                    }
                    else
                    {
                        DataCache.Unlock(previous);
                        break;
                    }

                currentId = currentNode.Value.Page.FullPageId;
                while (this.cache.TryGetValue(currentId with { PageId = currentId.PageId.Next }, out var next) && DataCache.TryLock(next))
                    if (!next.Invalid && next.GenerationFlag == currentFlag && next.Page.IsDirty)
                    {
                        gatherList.AddLast(next);
                        next.GenerationFlag = !currentFlag;
                        currentId = next.Page.FullPageId;
                    }
                    else
                    {
                        DataCache.Unlock(next);
                        break;
                    }

                // TODO: write gather list to log

                foreach (var slot in gatherList)
                {
                    await slot.Page.FlushAsync();
                    DataCache.Unlock(slot);
                }
            }
            // ReSharper disable once AccessToModifiedClosure - completes synchronously
            var nextNode = DataCache.TryLockValid(() => currentNode.Next);

            var isDone = nextNode == null || (currentNode = nextNode) == this.allocated.First;
                
            DataCache.Unlock(currentNode.Value);
            
            if (isDone)
                return;
        }
    }

    public readonly record struct Options(
        long CorrelatedReferencePeriod,
        long MaxSecondaryCorrelationPeriod,
        int MinimumFreeBuffers,
        int GrowBufferAllocationCount,
        int LazyWriterInterval,
        int LazyWriterSlotBatchSize,
        int LazyWriterSleepInterval
    );

    private record Slot(BufferPool.Buffer Buffer, IPage Page)
    {
        public int WriteLock;

        public Slot(BufferPool.Buffer buffer, IPage page, long ticks, bool generationFlag) : this(buffer, page)
        {
            this.GenerationFlag = generationFlag;
            this.LastAccess = this.UncorrelatedAccess1 = ticks;
        }

        public bool GenerationFlag { get; set; }
        public long LastAccess { get; private set; }
        public long UncorrelatedAccess1 { get; set; }
        public long UncorrelatedAccess2 { get; private set; }
        public bool Invalid { get; set; }

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

public interface IPage
{
    bool IsDirty { get; }
    FullPageId FullPageId { get; }
    Task FlushAsync();
}