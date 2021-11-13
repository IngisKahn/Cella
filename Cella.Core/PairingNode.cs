namespace Cella.Core;

public class PairingNode<T> where T : IComparable<T>
{
    public PairingNode(T element) => this.Element = element;

    public T Element { get; }
    public PairingNode<T>? Child { get; set; }
    public PairingNode<T>? Previous { get; set; }
    public PairingNode<T>? Next { get; set; }
}