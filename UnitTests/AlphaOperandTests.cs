using Tyke.Net;
using Tyke.Net.Data;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class AlphaOperandTests
{
    private readonly DatafieldAlpha _datafield;
    private readonly OperandAlpha _operand;

    public AlphaOperandTests()
    {
        Tyke.Net.Tyke.Context = new Context { ThrowOnError = true };

        var buffer = new DataBuffer(1024);
        _datafield = new DatafieldAlpha(buffer, "a", "1", "10");

        _operand = new OperandAlpha();
    }

    [Fact]
    public void OperandAlpha_Value_Equals_Datafield_Value()
    {
        _datafield.Set("AAA");
        _operand.SetFromToken("\"AAA\"");

        // Tyke compares the Datafield with the Operand
        // so GT is datafield > operand etc

        Assert.True(_operand.CompareEq(_datafield), "EQ");
        Assert.False(_operand.CompareLt(_datafield), "LT");
        Assert.False(_operand.CompareGt(_datafield), "GT");
        Assert.True(_operand.CompareGe(_datafield), "GE");
        Assert.True(_operand.CompareLe(_datafield), "LE");
        Assert.False(_operand.CompareNe(_datafield), "NE");
    }

    [Fact]
    public void OperandAlpha_Value_Greater_Than_Datafield_Value()
    {
        _datafield.Set("AAA");
        _operand.SetFromToken("\"BBB\"");

        // Tyke compares the Datafield with the Operand
        // so GT is datafield > operand etc

        Assert.False(_operand.CompareEq(_datafield), "EQ");
        Assert.True(_operand.CompareLt(_datafield), "LT");
        Assert.False(_operand.CompareGt(_datafield), "GT");
        Assert.False(_operand.CompareGe(_datafield), "GE");
        Assert.True(_operand.CompareLe(_datafield), "LE");
        Assert.True(_operand.CompareNe(_datafield), "NE");
    }

    [Fact]
    public void OperandAlpha_Value_Less_Than_Datafield_Value()
    {
        _datafield.Set("BBB");
        _operand.SetFromToken("\"AAA\"");

        // Tyke compares the Datafield with the Operand
        // so GT is datafield > operand etc

        Assert.False(_operand.CompareEq(_datafield), "EQ");
        Assert.False(_operand.CompareLt(_datafield), "LT");
        Assert.True(_operand.CompareGt(_datafield), "GT");
        Assert.True(_operand.CompareGe(_datafield), "GE");
        Assert.False(_operand.CompareLe(_datafield), "LE");
        Assert.True(_operand.CompareNe(_datafield), "NE");
    }
}