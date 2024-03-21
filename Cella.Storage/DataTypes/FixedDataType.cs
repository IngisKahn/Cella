namespace Cella.Storage.DataTypes;

public abstract class FixedDataType<T>(int length) : DataType<T>
{
    public override int MaxLength { get; } = length;
    public abstract T Read(ReadOnlySpan<byte> data);
    public abstract void Write(Span<byte> data, in T value);
}