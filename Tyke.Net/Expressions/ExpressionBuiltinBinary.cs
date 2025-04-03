namespace Tyke.Net.Expressions;

internal class ExpressionBuiltinBinary : ExpressionBuiltinBase
{
    private Data.DatafieldBinary _datafield;

    internal ExpressionBuiltinBinary(Parser.Tokeniser stack)
    {
        ParseExpression(stack);
    }

    internal sealed override void ParseExpression(Parser.Tokeniser stack)
    {	
        // must be 3 tokens
        if (!stack.VerifyCount(3))
            return;

        // next is datafield
        _datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBinary>(stack.Pop());
        if (_datafield == null)
            return;
                        
        // is or not
        GetTestType(stack.Pop());

        // next is builtin test
        switch (stack.Pop())
        {
            case "max":
                Func = _datafield.IsMax;
                break;
            default:
                Errors.Error.SyntaxError("Unknown builtin test");
                break;
        }
    }
}