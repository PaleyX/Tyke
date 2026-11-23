using System;

namespace Tyke.Net.Assign;

internal enum OperatorTypesB
{
    Multiply,
    Divide,
    Mod,
    Plus,
    Minus,
    OpenBracket,
    CloseBracket
}

internal class BinaryAssignOperator : BinaryAssignBase
{
    internal BinaryAssignOperator(string element)
    {
        Operator = element switch
        {
            "+" => OperatorTypesB.Plus,
            "-" => OperatorTypesB.Minus,
            "\\" => OperatorTypesB.Divide,
            "%" => OperatorTypesB.Mod,
            "*" => OperatorTypesB.Multiply,
            "(" => OperatorTypesB.OpenBracket,
            ")" => OperatorTypesB.CloseBracket,
            _ => throw new ApplicationException("Bad operator in binary assign: [" + element + "]")
        };
    }

    internal OperatorTypesB Operator { get; }

    internal int Order()
    {
        return Operator switch
        {
            OperatorTypesB.Plus or OperatorTypesB.Minus => 1,
            OperatorTypesB.Multiply or OperatorTypesB.Divide => 2,
            OperatorTypesB.Mod => 3,
            OperatorTypesB.OpenBracket or OperatorTypesB.CloseBracket => 0,
            _ => throw new ApplicationException("BinaryAssignNumeric::Order")
        };
    }
}