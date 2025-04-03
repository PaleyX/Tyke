namespace Tyke.Net.Expressions;

internal class ExpressionRange<T, TO> : ExpressionBase where T: Data.DatafieldBase where TO: Data.OperandBase
{
    private T _test;
    private TO _start;
    private TO _end;


    internal ExpressionRange(Parser.Tokeniser stack)
    {
        ParseExpression(stack);
    }

    internal override bool Evaluate()
    {
        return _start.CompareGe(_test) && _end.CompareLe(_test);
    }

    internal sealed override void ParseExpression(Parser.Tokeniser stack)
    {
        // must be 5 tokens
        if (!stack.VerifyCount(5))
            return;

        // datafield
        _test = Symbols.SymbolTable.GetSymbol<T>(stack.Pop());
        if (_test == null)
            return;

        // inrange
        stack.VerifyAndPop("inrange");

        // literal or datafield
        _start = Data.OperandHelpers.AllocateOpFromToken(stack.Pop()) as TO;
        if (_start == null)
            Errors.Error.SyntaxError(Errors.StdErrors.IncompatibleOperands);

        // to
        stack.VerifyAndPop("to");

        // literal or datafield
        _end = Data.OperandHelpers.AllocateOpFromToken(stack.Pop()) as TO;
        if (_end == null)
            Errors.Error.SyntaxError(Errors.StdErrors.IncompatibleOperands);
    }
}