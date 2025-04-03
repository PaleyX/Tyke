using Tyke.Net.Data;

namespace UnitTests;

public class DatafieldTests
{
    [Theory]
    [InlineData("2")]
    [InlineData("4")]
    public void IncrementWorksOnBinary(string length)
    {
        var dataBuffer = new DataBuffer(1024);
        var dataField = new DatafieldBinary(dataBuffer, "test", "10", length);
        dataField.SetConstant("10");

        for (var i = 0; i < 1000; ++i)
        {
            dataField.Increment();
        }

        Assert.Equal(1010u, dataField.GetDWord());
    }
}