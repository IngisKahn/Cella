namespace Cella.Core.Tests;

using DataTypes;
using Xunit;

public class DataTypeTests
{
    [Theory]
    [InlineData(4, "test", "test")]
    [InlineData(6, "test", "test\0\0")]
    [InlineData(6, "test¿", "test¿")]
    [InlineData(3, "test", "tes")]
    [InlineData(5, "test¿", "test\0")]
    [InlineData(5, "¿¿¿¿¿", "¿¿\0")]
    public void FixedStringTests(int length, string input, string output)
    {
        FixedStringDataType dt = new(length);
        var data = new byte[length];
        dt.Write(data, ref input);
        Assert.Equal(output, dt.Read(data));
    }
}