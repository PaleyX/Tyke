namespace Tyke.Net.Process;

internal class CommandIfNot() : CommandBranchBase(CommandTypes.IfNot)
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyAndPop("ifnot");

        Symbols.ProcessStack.Push(this);

        Expression = Expressions.ExpressionBase.GetExpression(stack);
    }

    internal override CommandBase Process()
    {
        return Expression.Evaluate() ? Jump : Next;
    }
}