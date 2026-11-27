using Tyke.Net;
using Tyke.Net.Data;

namespace UnitTests;

[Collection("Non-Parallel Collection")]
public class BinaryOperandTests
{
    private readonly DatafieldBinary _datafield;
    private readonly OperandBinary _operand;

    public BinaryOperandTests()
    {
        Tyke.Net.Tyke.Context = new Context { ThrowOnError = true };

        var buffer = new DataBuffer(1024);
        _datafield = new DatafieldBinary(buffer, "a", "1", "4");

        _operand = new OperandBinary();
    }

    [Fact]
    public void OperandBinary_Value_Equals_Datafield_Value()
    {
        _datafield.Set("986");
        _operand.SetFromToken("986");

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
    public void OperandBinary_Value_Greater_Than_Datafield_Value()
    {
        _datafield.Set("986");
        _operand.SetFromToken("1003");

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
    public void OperandBinary_Value_Less_Than_Datafield_Value()
    {
        _datafield.Set("1003");
        _operand.SetFromToken("986");

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