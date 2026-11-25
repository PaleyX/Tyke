using Tyke.Net;
using Tyke.Net.Data;
using Tyke.Net.Parser;
using Tyke.Net.Expressions;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class ExpressionsTests
{
    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(4, false)]
    [InlineData(5, true)]
    public void Expression_Binary_List_Works(int value, bool expected)
    {
        Tyke.Net.Tyke.Context = new Context();

        var buffer = new DataBuffer(1024);

        var datafield = new DatafieldBinary(buffer, "a", "1", "4");
        datafield.Set((uint)value);

        var tokeniser = new Tokeniser("a in 1,2,3,5");
        var expression = new ExpressionList<DatafieldBinary, OperandBinary>(tokeniser);

        Assert.Equal(expected, expression.Evaluate());
    }

    [Theory]
    [InlineData("a", false)]
    [InlineData("b", true)]
    [InlineData("c", true)]
    [InlineData("d", true)]
    [InlineData("e", false)]
    [InlineData("f", true)]
    public void Expression_Alpha_List_Works(string value, bool expected)
    {
        Tyke.Net.Tyke.Context = new Context();

        var buffer = new DataBuffer(1024);

        var datafield = new DatafieldAlpha(buffer, "a", "1", "4");
        datafield.Set(value);

        var tokeniser = new Tokeniser("""a in "b","c","d","f" """);
        var expression = new ExpressionList<DatafieldAlpha, OperandAlpha>(tokeniser);

        Assert.Equal(expected, expression.Evaluate());
    }
}
