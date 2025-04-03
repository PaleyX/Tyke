using System;

namespace Tyke.Net.Data;

internal abstract class OperandBase
{
    internal enum OperandTypes
    {
        Numeric,
        Alpha
    }

    internal enum DataTypes
    {
        Undefined = 0,
        Datafield = 1,
        Constant  = 2
    }

    private DataTypes _dataType;

    internal OperandBase(OperandTypes operandType)
    {
        _dataType = DataTypes.Undefined;
        OperandType = operandType;
    }

    internal DataTypes DataType => _dataType;

    // have to do this because I can't work out how to do a protected set and an internal get on a property
    protected void SetDataType(DataTypes dataType)
    {
        _dataType = dataType;
    }

    protected void AssertDataType()
    {
        if(!Valid())
            throw new ApplicationException("OperandBase::AssertDataType");
    }

    private OperandTypes OperandType {get; set;}

    internal abstract void SetFromToken(string token, bool mustBeDatafield = false);
    internal abstract bool CompareEq(DatafieldBase datafield);
    internal abstract bool CompareNe(DatafieldBase datafield);
    internal abstract bool CompareGt(DatafieldBase datafield);
    internal abstract bool CompareLt(DatafieldBase datafield);
    internal abstract bool CompareGe(DatafieldBase datafield);
    internal abstract bool CompareLe(DatafieldBase datafield);

    public bool Valid()  
    {
        return DataType == DataTypes.Datafield || DataType == DataTypes.Constant;
    }

    //public bool IsCompatible(DatafieldBase datafield)
    //{
    //    AssertDataType();

    //    if(datafield is DatafieldBinary)
    //        return OperandType == OperandTypes.Numeric;

    //    if(datafield is DatafieldAlpha)
    //        return OperandType == OperandTypes.Alpha;

    //    throw new ApplicationException("Unknown datafield type in OperandBase::IsCompatible");
    //}

    public bool IsConstant () 
    {
        AssertDataType(); 
        return DataType == DataTypes.Constant;
    }

    public bool IsDatafield() 
    {
        AssertDataType(); 
        return DataType == DataTypes.Datafield;
    }
}