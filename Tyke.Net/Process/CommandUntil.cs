using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("until")]
internal class CommandUntil : CommandBranchBase
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