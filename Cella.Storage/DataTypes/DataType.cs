namespace Cella.Storage.DataTypes;

using System.Runtime.InteropServices;
using System.Text;

public abstract class DataType<T>
{
    public abstract int MaxLength { get; }

    protected static (byte[] Bytes, int Characters) ConvertString(int maxLength, string value)
    {
        var chars = value.ToCharArray();
        var stringLength = Math.Min(chars.Length, maxLength);
        var byteCount = Encoding.UTF8.GetByteCount(chars, 0, stringLength);

        (byte[], int) Convert()
        {
            var bytes = new byte[byteCount];
            Encoding.UTF8.GetBytes(chars, 0, stringLength, bytes, 0);
            return (bytes, stringLength);
        }

        // simple case, it fits
        if (byteCount <= maxLength)
            return Convert();

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
        do
        {
            if (searchLength > 0)
                Search(ref maxLength, ref byteCount);
            else
                Search(ref byteCount, ref maxLength);
            searchLength = -searchLength;
        } while (searchLength != 0);
        
        return Convert();
    }
}

//
public class StringDataType : DataType<string>
{
    public int Read(ReadOnlySpan<byte> data, out string value)
    {
        var length = MemoryMarshal.Read<ushort>(data);
        value = Encoding.UTF8.GetString(data[2..]);
        return length;
    }

    public int Write(Span<byte> data, string value)
    {
        var (bytes, _) = DataType<string>.ConvertString(this.MaxLength - 2, value);

        var byteCountField = (ushort)bytes.Length;
        MemoryMarshal.Write(data, ref byteCountField);
        bytes.CopyTo(data[2..]);
        return byteCountField;
    }

    public override int MaxLength { get; }
}

public abstract class FixedDataType<T> : DataType<T>
{
    public override int MaxLength { get; }
    protected FixedDataType(int length) => this.MaxLength = length;
    public abstract T Read(ReadOnlySpan<byte> data);
    public abstract void Write(Span<byte> data, ref T value);
}

public unsafe class IntrinsicDataType<T> : FixedDataType<T> where T : unmanaged
{
    public IntrinsicDataType() : base(sizeof(T)) { }

    public override T Read(ReadOnlySpan<byte> data) => MemoryMarshal.Read<T>(data);

    public override void Write(Span<byte> data, ref T value) => MemoryMarshal.Write(data, ref value);
}

public class FixedStringDataType : FixedDataType<string>
{
    public FixedStringDataType(int length) : base(length) { }

    public override string Read(ReadOnlySpan<byte> data) => Encoding.UTF8.GetString(data);

    public override void Write(Span<byte> data, ref string value)
    {
        var (bytes, _) = DataType<string>.ConvertString(this.MaxLength, value);
        
        bytes.CopyTo(data);
    }
}
