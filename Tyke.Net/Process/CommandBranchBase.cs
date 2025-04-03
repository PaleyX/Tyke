namespace Tyke.Net.Process;

internal abstract class CommandBranchBase : CommandBase
{
    protected Expressions.ExpressionBase Expression;


    internal CommandBranchBase(CommandTypes commandType)
        : base(commandType)
    {
    }

    internal CommandBase Jump { get; set; }
}