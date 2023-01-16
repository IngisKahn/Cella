namespace Cella.Storage.DataTypes;

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