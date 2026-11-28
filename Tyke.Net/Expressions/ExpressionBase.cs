using Tyke.Net.Data;

namespace Tyke.Net.Expressions;

internal abstract class ExpressionBase 
{
    //internal enum ExpressionTypes
    //{
    //    Unknown,
    //    Indicator,
    //    List,
    //    Simple,
    //    Range,
    //    Select,
    //    IsBool
    //}

    internal abstract bool Evaluate();
    internal abstract void ParseExpression(Parser.Tokeniser stack);

    internal static ExpressionBase GetExpression(Parser.Tokeniser stack)
    {
        if (stack.Count() == 0)
        {
            Errors.Error.SyntaxError();
            return null;
        }

        // if only one token has to be an indicator
        if (stack.Count() == 1)
            return new ExpressionIndicator(stack);

        // cancast?
        if (stack.Peek() == "cancast")
            return new ExpressionCanCast(stack);

        // has to be datafield
        var datafield = Symbols.SymbolTable.GetSymbol<DatafieldBase>(stack.Peek());
        if (datafield == null)
            return null;

        // is or not
        if (stack.Peek(2) == "is" || stack.Peek(2) == "not")
        {
            switch (datafield)
            {
                case DatafieldAlpha:
                    return new ExpressionBuiltinAlpha(stack);
                case DatafieldBinary:
                    return new ExpressionBuiltinBinary(stack);
                default:
                    Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
                    break;
            }
        }

        // in?
        if (stack.Peek(2) == "in")
        {
            switch (datafield)
            {
                case DatafieldAlpha:
                    return new ExpressionList<DatafieldAlpha, OperandAlpha>(stack);
                case DatafieldBinary:
                    return new ExpressionList<DatafieldBinary, OperandBinary>(stack);
                default:
                    Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
                    return null;
            }
        }

        // inrange
        if (stack.Peek(2) == "inrange")
        {
            switch (datafield)
            {
                case DatafieldAlpha:
                    return new ExpressionRange<DatafieldAlpha, OperandAlpha>(stack);
                case DatafieldBinary:
                    return new ExpressionRange<DatafieldBinary, OperandBinary>(stack);
                default:
                    Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
                    return null;
            }
        }

        // must be simple
        if(datafield is DatafieldAlpha)
        {
            return new ExpressionSimple<DatafieldAlpha, OperandAlpha>(stack);
        }

        return new ExpressionSimple<DatafieldBinary, OperandBinary>(stack);
    }

    internal static ExpressionBase GetExpression(string line)
    {
        var tokeniser = new Parser.Tokeniser(line);

        return GetExpression(tokeniser);

    }
}