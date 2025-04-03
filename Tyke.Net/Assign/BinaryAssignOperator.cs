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
        switch (element)
        {
            case "+":
                Operator = OperatorTypesB.Plus;
                break;
            case "-":
                Operator = OperatorTypesB.Minus;
                break;
            case "\\":
                Operator = OperatorTypesB.Divide;
                break;
            case "%":
                Operator = OperatorTypesB.Mod;
                break;
            case "*":
                Operator = OperatorTypesB.Multiply;
                break;
            case "(":
                Operator = OperatorTypesB.OpenBracket;
                break;
            case ")":
                Operator = OperatorTypesB.CloseBracket;
                break;
            default:
                throw new ApplicationException("Bad operator in binary assign: ["  + element + "]");
        }
    }

    internal OperatorTypesB Operator { get; }

    internal int Order()
    {
        switch (Operator)
        {
            case OperatorTypesB.Plus:
            case OperatorTypesB.Minus:
                return 1;
            case OperatorTypesB.Multiply:
            case OperatorTypesB.Divide:
                return 2;
            case OperatorTypesB.Mod:
                return 3;
            case OperatorTypesB.OpenBracket:
            case OperatorTypesB.CloseBracket:
                return 0;
        }

        throw new ApplicationException("BinaryAssignNumeric::Order");
    }
}