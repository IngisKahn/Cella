namespace Cella.Core.Tests
{
    using System;
    using Xunit;

    public class PairingHeapTests
    {
        [Fact]
        public void AddOneItem()
        {
            PairingHeap<int> heap = new();
            Assert.Equal(0, heap.Count);
            Assert.Null(heap.MinimumNode);
            heap.Add(11); 
            Assert.Equal(1, heap.Count);
            Assert.NotNull(heap.MinimumNode);
            Assert.Equal(11, heap.MinimumNode!.Element);
        }

        [Fact]
        public void MinimumOfThree()
        {
            PairingHeap<int> heap = new();
            heap.Add(111);
            heap.Add(11);
            heap.Add(12);
            Assert.Equal(11, heap.MinimumNode!.Element);
            Assert.Equal(3, heap.Count);
        }

        [Fact]
        public void RemoveMin()
        {
            PairingHeap<int> heap = new();
            heap.Add(111);
            heap.Add(11);
            heap.Add(12);
            heap.RemoveMinimum();
            Assert.Equal(12, heap.MinimumNode!.Element);
            Assert.Equal(2, heap.Count);
        }

        [Fact]
        public void Remove()
        {
            PairingHeap<int> heap = new();
            heap.Add(111);
            heap.Add(11);
            var n = heap.Add(12);
            heap.Remove(n);
            heap.RemoveMinimum();
            Assert.Equal(111, heap.MinimumNode!.Element);
            Assert.Equal(1, heap.Count);
        }

        public class IntWrapper : IComparable<IntWrapper>
        {
            public int Value;
            private IntWrapper(int value) => this.Value = value;
            public static implicit operator int(IntWrapper wrapper) => wrapper.Value;
            public static implicit operator IntWrapper(int value) => new(value);

            public int CompareTo(IntWrapper? other) => this.Value.CompareTo(other?.Value);
        }

        [Fact]
        public void DecreaseKey()
        {
            PairingHeap<IntWrapper> heap = new();
            var n = heap.Add(111);
            heap.Add(11);
            heap.Add(12);
            n.Element.Value = 2;
            heap.KeyDecreased(n);
            Assert.Equal(2, (int)heap.MinimumNode!.Element);
            Assert.Equal(3, heap.Count);
        }
    }
}
