namespace Tyke.Net.Expressions
{
    internal class ExpressionBuiltinAlpha : ExpressionBuiltinBase
    {
        private Data.DatafieldAlpha _datafield;

        internal ExpressionBuiltinAlpha(Parser.Tokeniser stack)
        {
            ParseExpression(stack);
        }

        internal sealed override void ParseExpression(Parser.Tokeniser stack)
        {	
            // must be 3 tokens
            if (!stack.VerifyCount(3))
                return;

            // next is datafield
            _datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(stack.Pop());
            if (_datafield == null)
                return;

            // is or not
            GetTestType(stack.Pop());

            // next is builtin test
            switch (stack.Pop())
            {
                //case "numeric":
                //    _Func = _Datafield.IsNumeric;
                //    break;
                //case "alphabetic":
                //    _Func = _Datafield.IsAlphabetic;
                //    break;
                default:
                    Errors.Error.SyntaxError("Unknown builtin test");
                    break;
            }
        }
    }
}
