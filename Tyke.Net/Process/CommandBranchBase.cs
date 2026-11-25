namespace Tyke.Net.Process;

internal abstract class CommandBranchBase : CommandBase
{
    protected Expressions.ExpressionBase Expression;


    internal CommandBase Jump { get; set; }
}