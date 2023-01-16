namespace Cella.Storage.DataTypes;

public abstract class FixedDataType<T> : DataType<T>
{
    public override int MaxLength { get; }
    protected FixedDataType(int length) => this.MaxLength = length;
    public abstract T Read(ReadOnlySpan<byte> data);
    public abstract void Write(Span<byte> data, ref T value);
}