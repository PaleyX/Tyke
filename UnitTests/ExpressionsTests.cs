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
    public void Gort(int value, bool expected)
    {
        Tyke.Net.Symbols.SymbolTable.Clear();

        var buffer = new DataBuffer(1024);

        var datafield = new DatafieldBinary(buffer, "a", "1", "4");
        datafield.Set((uint)value);

        var tokeniser = new Tokeniser("a in 1,2,3,5");
        var expression = new ExpressionList<DatafieldBinary, OperandBinary>(tokeniser);

        Assert.Equal(expected, expression.Evaluate());
    }
}
