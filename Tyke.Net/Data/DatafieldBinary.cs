using System;
using System.Diagnostics;

namespace Tyke.Net.Data;

internal class DatafieldBinary : DatafieldBase
{
    internal DatafieldBinary(DataBuffer area, string name, string offset, string length)
        : base(area, name, offset, length, DatafieldTypes.Binary)
    {
    }

    protected override bool ValidateFieldLengthForType() => Length is 2 or 4;

    internal override void SetConstant(string constant)
    {
        try
        {
            if (Length == 2)
            {
                var value = ushort.Parse(constant);
                Area.Insert(value, Offset);
                return;
            }

            if (Length == 4)
            {
                var value = uint.Parse(constant);
                Area.Insert(value, Offset);
                return;
            }

            Errors.Error.SyntaxError("Invalid binary field length");
        }
        catch(Exception)
        {
            Errors.Error.SyntaxError("Invalid constant value");
        }
    }

    internal void Increment()
    {
        switch (Length)
        {
            case 2:
                Area.Increment<ushort>(Offset);
                break;
            case 4:
                Area.Increment<uint>(Offset);
                break;
        }
    }

    internal uint GetDWord()
    {
        if (Length == 4)
        {
            return Area.GetBinaryU4(Offset);
        }

        Debug.Assert(Length == 2);
        return Area.GetBinaryU2(Offset);
    }

    internal void Set(OperandBinary operand)
    {
        Set(operand.GetDWord());
    }

    internal void Set(uint value)
    {
        if (Length == 4)
        {
            Area.Insert(value, Offset);
            return;
        }

        if (Length == 2)
        {
            var num = Convert.ToUInt16(value);
            Area.Insert(num, Offset);
            return;
        }

        Debug.Assert(false);
    }

    internal override void Set(string value)
    {
        try
        {
            uint number = uint.Parse(value);
            Set(number);
        }
        catch(Exception e)
        {
            Errors.Error.ReportError("Error setting binary datafield: " + e.Message);
        }
    }

    internal override bool CanCast(DatafieldBase datafield)
    {
        return datafield is DatafieldAlpha;
    }

    internal override void Cast(DatafieldBase datafield)
    {
        var x = datafield as DatafieldAlpha;
        Debug.Assert(x != null);

        string value = ToString();

        if (value.Length > x.Length)
            Errors.Error.ReportError("Overflow casting binary to alpha");

        value = value.PadLeft(x.Length, '0');

        x.Set(value);
    }

    internal void SetMax()
    {
        switch (Length)
        {
            case 4:
                Area.Insert(uint.MaxValue, Offset);
                break;
            case 2:
                Area.Insert(ushort.MaxValue, Offset);
                break;
            default:
                throw new TykeRunTimeException("Uknown length in SetMax()");
        }
    }

    internal bool IsMax()
    {
        switch (Length)
        {
            case 4:
                return GetDWord() == uint.MaxValue;
            case 2:
                return ((ushort)GetDWord()) == ushort.MaxValue;
        }

        throw new TykeRunTimeException("Unknown length in IsMax())");
    }

    public override string ToString()
    {
        return GetDWord().ToString();
    }
}