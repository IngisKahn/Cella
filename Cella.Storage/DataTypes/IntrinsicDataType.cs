namespace Cella.Storage.DataTypes;

using System.Runtime.InteropServices;

public unsafe class IntrinsicDataType<T> : FixedDataType<T> where T : unmanaged
{
    public IntrinsicDataType() : base(sizeof(T)) { }

    public override T Read(ReadOnlySpan<byte> data) => MemoryMarshal.Read<T>(data);

    public override void Write(Span<byte> data, ref T value) => MemoryMarshal.Write(data, ref value);
}