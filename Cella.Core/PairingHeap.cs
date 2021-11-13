namespace Cella.Core;

public class PairingHeap<T>
    where T : IComparable<T>
{
    public int Count { get; private set; }

    public PairingNode<T>? MinimumNode { get; private set; }

    private void Meld(PairingNode<T>? node)
    {
        if (this.MinimumNode == null)
        {
            this.MinimumNode = node;
            return;
        }

        this.MinimumNode = PairingHeap<T>.Meld(this.MinimumNode, node);
    }

    public PairingNode<T> Add(T element)
    {
        PairingNode<T> node = new(element);
        this.Meld(node);
        this.Count++;
        return node;
    }

    private static void Prune(PairingNode<T> node)
    {
        if (node.Previous!.Child == node) 
            node.Previous.Child = node.Next;
        else
            node.Previous.Next = node.Next;
        if (node.Next != null)
            node.Next.Previous = node.Previous;
        node.Previous = node.Next = null;
    }

    public void KeyDecreased(PairingNode<T> node)
    {
        if (node == this.MinimumNode)
            return;
        PairingHeap<T>.Prune(node);
        this.Meld(node);
    }

    public PairingNode<T> RemoveMinimum()
    {
        var min = this.MinimumNode ?? throw new InvalidOperationException("No nodes to remove");
        this.Count--;
        if (min.Child != null)
            min.Child.Previous = null;
        this.MinimumNode = PairingHeap<T>.MergePairs(min.Child);

        return min;
    }

    private static PairingNode<T>? MergePairs(PairingNode<T>? first) =>
        first == null ? null :
        first.Next == null ? first :
        PairingHeap<T>.Meld(PairingHeap<T>.MergePairs(first.Next.Next), PairingHeap<T>.Meld(first, first.Next));

    private static PairingNode<T>? Meld(PairingNode<T>? nodeA, PairingNode<T>? nodeB)
    {
        if (nodeA == null)
            return nodeB;
        if (nodeB == null)
            return nodeA;

        if (nodeA.Element.CompareTo(nodeB.Element) < 0)
            (nodeA, nodeB) = (nodeB, nodeA);

        nodeA.Next = nodeB.Child;
        if (nodeA.Next != null)
            nodeA.Next.Previous = nodeA;
        nodeA.Previous = nodeB;
        nodeB.Child = nodeA;
        return nodeB;
    }

    public void Remove(PairingNode<T> node)
    {
        if (node == this.MinimumNode)
        {
            this.RemoveMinimum();
            return;
        }

        this.Count--;
        PairingHeap<T>.Prune(node);
        if (node.Child == null)
            return;
        node.Child.Previous = null;
        node = PairingHeap<T>.MergePairs(node.Child)!;
        node.Next = this.MinimumNode!.Child;
        if (node.Next != null)
            node.Next.Previous = node;
        node.Previous = this.MinimumNode;
        this.MinimumNode.Child = node;
    }
}