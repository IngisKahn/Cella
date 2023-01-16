namespace Cella.Storage.DataTypes;

using System.Runtime.InteropServices;
using System.Text;

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