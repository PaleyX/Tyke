using System.Diagnostics;

namespace Tyke.Net.Data;

internal class OperandBinary : OperandBase
{
    private DatafieldBinary _datafield;
    private uint _const;

    public OperandBinary() 
        : base(OperandTypes.Numeric)
    {
    }

    internal OperandBinary(DatafieldBinary datafield) 
        : base(OperandTypes.Numeric)
    {
        _datafield = datafield;

        SetDataType(DataTypes.Datafield);
    }

    #region Comparisons
    internal override bool CompareEq(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() == GetDWord();
    }

    internal override bool CompareNe(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() != GetDWord();
    }

    internal override bool CompareGt(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() > GetDWord();
    }

    internal override bool CompareGe(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() >= GetDWord();
    }

    internal override bool CompareLe(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() <= GetDWord();
    }

    internal override bool CompareLt(DatafieldBase datafield)
    {
        return GetBinaryForComparison(datafield).GetDWord() < GetDWord();
    }

    private static DatafieldBinary GetBinaryForComparison(DatafieldBase datafield)
    {
        if (datafield is DatafieldBinary binary)
        {
            return binary;
        }

        throw new TykeRunTimeException("Expected binary datafield for comparison");
    }
    #endregion Comparisons

    internal override void SetFromToken(string token, bool mustBeDatafield = false)
    {
        Debug.Assert(DataType == DataTypes.Undefined);

        var datafield = Symbols.SymbolTable.GetSymbol<DatafieldBinary>(token, false);
        if(datafield != null)
        {
            SetDataType(DataTypes.Datafield);
            _datafield = datafield;
            return;
        }

        if(mustBeDatafield)
        {
            Errors.Error.SyntaxError("Expected binary datafield");
            return;
        }

        if(Tools.StringTools.CanConvert<uint>(token))
        {
            SetDataType(DataTypes.Constant);
            _const = Tools.StringTools.Convert<uint>(token);
        }
        else
            Errors.Error.SyntaxError("Expected binary datafield or literal");
    }

    internal uint GetDWord()
    {
        if (_datafield == null)
            return _const;
        else
            return _datafield.GetDWord();
    }
}