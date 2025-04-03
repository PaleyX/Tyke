using System.Diagnostics;

namespace Tyke.Net.Data;

internal class OperandAlpha : OperandBase
{
    private DatafieldAlpha _datafield;
    private string _const;

    public OperandAlpha() 
        : base(OperandTypes.Alpha)
    {
    }

    internal OperandAlpha(DatafieldAlpha datafield)
        : base(OperandTypes.Alpha)
    {
        _datafield = datafield;
        SetDataType(DataTypes.Datafield);
    }

    #region Comparisons
    internal override bool CompareEq(DatafieldBase datafield)
    {
        return Compare(datafield) == 0;
    }

    internal override bool CompareNe(DatafieldBase datafield)
    {
        return Compare(datafield) != 0;
    }

    internal override bool CompareGt(DatafieldBase datafield)
    {
        return Compare(datafield) > 0;
    }

    internal override bool CompareGe(DatafieldBase datafield)
    {
        return Compare(datafield) >= 0;
    }

    internal override bool CompareLe(DatafieldBase datafield)
    {
        return Compare(datafield) <= 0;
    }

    internal override bool CompareLt(DatafieldBase datafield)
    {
        return Compare(datafield) < 0;
    }

    private int Compare(DatafieldBase datafield)
    {
        Debug.Assert(datafield is DatafieldAlpha);

        return string.Compare((datafield as DatafieldAlpha).ToString(), ToString());
    }
    #endregion Comparisons

    internal override void SetFromToken(string token, bool mustBeDatafield = false)
    {
        Debug.Assert(DataType == DataTypes.Undefined);

        var datafield = Symbols.SymbolTable.GetSymbol<DatafieldAlpha>(token, false);
        if (datafield != null)
        {
            SetDataType(DataTypes.Datafield);
            _datafield = datafield;
            return;
        }

        if (mustBeDatafield)
        {
            Errors.Error.SyntaxError("Expected alpha datafield");
            return;
        }

        if (Tools.StringTools.IsQuotedString(token))
        {
            SetDataType(DataTypes.Constant);
            _const = Tools.StringTools.GetQuotedString(token);
        }
        else
            Errors.Error.SyntaxError("Expected alpha datafield or literal");
    }

    public override string ToString()
    {
        if (DataType == DataTypes.Datafield)
            return _datafield.ToString();
        else
            return _const;
    }

}