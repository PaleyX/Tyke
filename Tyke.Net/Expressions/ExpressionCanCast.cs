namespace Tyke.Net.Expressions;

internal class ExpressionCanCast : ExpressionBase
{
    private Data.DatafieldBase _source;
    private Data.DatafieldBase _object;

    internal ExpressionCanCast(Parser.Tokeniser stack)
    {
        ParseExpression(stack);
    }

    internal override bool Evaluate()
    {
        if (_source is Data.DatafieldAlpha alpha)
        {
            //TODO 2 or 4 byte binary
            return Tools.StringTools.CanConvert<uint>(alpha.ToString());
        }

        return true;
    }

    internal sealed override void ParseExpression(Parser.Tokeniser stack)
    {
        // must be 4 tokens#
        if (!stack.VerifyCount(4))
            return;

        // cancast
        stack.VerifyAndPop("cancast");

        // must be datafield
        _source = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());
        if (_source == null)
            return;

        // into
        stack.VerifyAndPop("into");

        // must be datafield
        _object = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());
        if (_object == null)
            return;

        // compatible>
        if (!_source.CanCast(_object))
            Errors.Error.SyntaxError("Incompatible casting operands");
    }
}