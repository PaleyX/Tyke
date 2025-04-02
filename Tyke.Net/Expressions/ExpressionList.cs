namespace Tyke.Net.Expressions
{
    internal class ExpressionList<T, TO> : ExpressionBase where T: Data.DatafieldBase where TO: Data.OperandBase
    {
        private T _test;
        private readonly Data.OperandList<TO> _opList = new();

        internal ExpressionList(Parser.Tokeniser stack)
        {
            ParseExpression(stack);
        }

        internal override bool Evaluate()
        {
            var list = _opList.GetList();

            foreach (TO op in list)
            {
                if (op.CompareEq(_test))
                    return true;
            }

            return false;
        }

        internal sealed override void ParseExpression(Parser.Tokeniser stack)
        {
            // 3 or more tokens
            if (stack.Count() < 3)
            {
                Errors.Error.SyntaxError();
                return;
            }

            // next is datafield
            _test = Symbols.SymbolTable.GetSymbol<T>(stack.Pop());
            if (_test == null)
                return;

            // next is IN
            stack.VerifyAndPop("in");

            // list
            _opList.ParseList(stack);
        }
    }
}
