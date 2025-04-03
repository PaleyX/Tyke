namespace Tyke.Net.Process;

internal class CommandIf() : CommandBranchBase(CommandTypes.If)
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyAndPop("if");

        Symbols.ProcessStack.Push(this);

        Expression = Expressions.ExpressionBase.GetExpression(stack);
    }

    internal override CommandBase Process()
    {
        return Expression.Evaluate() ? Next : Jump;
    }
}