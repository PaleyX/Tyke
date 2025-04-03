namespace Tyke.Net.Process;

internal class CommandUntil() : CommandBranchBase(CommandTypes.Until)
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyAndPop("until");

        Symbols.ProcessStack.Push(this);

        Expression = Expressions.ExpressionBase.GetExpression(stack);
    }

    internal override CommandBase Process()
    {
        return Expression.Evaluate() ? Jump : Next;
    }
}