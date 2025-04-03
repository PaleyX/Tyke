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
        var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Peek());
        if (datafield == null)
            return null;

        // is or not
        if (stack.Peek(2) == "is" || stack.Peek(2) == "not")
        {
            if(datafield is Data.DatafieldAlpha)
                return new ExpressionBuiltinAlpha(stack);

            if (datafield is Data.DatafieldBinary)
                return new ExpressionBuiltinBinary(stack);

            Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
        }

        // in?
        if (stack.Peek(2) == "in")
        {
            if (datafield is Data.DatafieldAlpha)
                return new ExpressionList<Data.DatafieldAlpha, Data.OperandAlpha>(stack);

            if (datafield is Data.DatafieldBinary)
                return new ExpressionList<Data.DatafieldBinary, Data.OperandBinary>(stack);

            Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
            return null;
        }

        // inrange
        if (stack.Peek(2) == "inrange")
        {
            if (datafield is Data.DatafieldAlpha)
                return new ExpressionRange<Data.DatafieldAlpha, Data.OperandAlpha>(stack);

            if(datafield is Data.DatafieldBinary)
                return new ExpressionRange<Data.DatafieldBinary, Data.OperandBinary>(stack);

            Errors.Error.SyntaxError(Errors.StdErrors.UnknownDatafieldType);
            return null;
        }

        // must be simple
        if(datafield is Data.DatafieldAlpha)
        {
            return new ExpressionSimple<Data.DatafieldAlpha, Data.OperandAlpha>(stack);
        }
        else
        {
            return new ExpressionSimple<Data.DatafieldBinary, Data.OperandBinary>(stack);
        }
    }

    internal static ExpressionBase GetExpression(string line)
    {
        var tokeniser = new Parser.Tokeniser(line);

        return GetExpression(tokeniser);

    }
}