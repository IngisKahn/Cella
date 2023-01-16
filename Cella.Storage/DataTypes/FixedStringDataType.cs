namespace Cella.Storage.DataTypes;

using System.Text;

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