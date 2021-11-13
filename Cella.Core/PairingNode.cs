namespace Cella.Core;

public record PairingNode<T>(T Element) where T : IComparable<T>
{
    public PairingNode<T>? Child { get; set; }
    public PairingNode<T>? Previous { get; set; }
    public PairingNode<T>? Next { get; set; }
}