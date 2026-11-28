using Tyke.Net.Data;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class DataBufferTests
{
    [Fact]
    public void Insert_String_Longer_Than_Field_Size_Truncates()
    {
        const string data = "Hello there, how are you";

        var buffer = new DataBuffer(1024);
        buffer.Insert(data, 0, 10);

        var inserted = buffer.GetString(0, 10);

        Assert.Equal(data[..10], inserted);
    }
}
