namespace Cella.Core.DataTypes;

using System.Runtime.InteropServices;
using System.Text;

public abstract class DataType<T>
{
    public abstract T Read(ReadOnlySpan<byte> data);
    public abstract void Write(Span<byte> data, ref T value);
    public abstract int Length { get; }
}

public abstract class FixedDataType<T> : DataType<T>
{
    public override int Length { get; }
    protected FixedDataType(int length) => this.Length = length;
}

public unsafe class IntrinsicDataType<T> : FixedDataType<T> where T : unmanaged
{
    public IntrinsicDataType() : base(sizeof(T)) { }

    public override T Read(ReadOnlySpan<byte> data) => MemoryMarshal.Read<T>(data);

    public override void Write(Span<byte> data, ref T value) => MemoryMarshal.Write(data, ref value);
}

public class StringDataType : FixedDataType<string>
{
    public StringDataType(int length) : base(length) { }

    public override string Read(ReadOnlySpan<byte> data) => Encoding.UTF8.GetString(data);

    public override void Write(Span<byte> data, ref string value)
    {
        var chars = value.ToCharArray();
        var stringLength = Math.Min(chars.Length, this.Length);
        var byteCount = Encoding.UTF8.GetByteCount(chars, 0, stringLength);

        void ConvertAndWrite(Span<byte> dest)
        {
            var bytes = new byte[byteCount];
            Encoding.UTF8.GetBytes(chars, 0, stringLength, bytes, 0);
            bytes.CopyTo(dest);
        }

        // simple case, it fits
        if (byteCount <= this.Length)
        {
            ConvertAndWrite(data);
            return;
        }

        var searchLength = -(stringLength >> 1);
        void Search(ref int a, ref int b)
        {
            var lastStringLength = 0;
            var lastByteCount = 0;
            while (a > b && searchLength != 0)
            {
                lastStringLength = stringLength; 
                lastByteCount = byteCount;
                stringLength += searchLength;
                searchLength >>= 1;
                byteCount = Encoding.UTF8.GetByteCount(chars!, 0, stringLength);
            }

            if (searchLength != 0 || a > b) 
                return;
            stringLength = lastStringLength;
            byteCount = lastByteCount;
        }
        
        var targetLength = this.Length;
        do
        {
            if (searchLength > 0)
                Search(ref targetLength, ref byteCount);
            else
                Search(ref byteCount, ref targetLength);
            searchLength = -searchLength;
        } while (searchLength != 0);
        
        ConvertAndWrite(data);
    }
}
