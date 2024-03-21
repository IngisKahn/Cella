namespace Cella.Storage.DataTypes;

using System.Runtime.InteropServices;

public unsafe class IntrinsicDataType<T>() : FixedDataType<T>(sizeof(T))
    where T : unmanaged
{
    public override T Read(ReadOnlySpan<byte> data) => MemoryMarshal.Read<T>(data);

    public override void Write(Span<byte> data, in T value) => MemoryMarshal.Write(data, in value);
}