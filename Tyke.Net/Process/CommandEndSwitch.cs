namespace Tyke.Net.Process;

internal class CommandEndSwitch() : CommandBase(CommandTypes.EndSwitch)
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyCount(1);
        stack.VerifyAndPop("endswitch");

        // pop process stack
        var x = Symbols.ProcessStack.Pop();
        if (x == null || x.CommandType != CommandTypes.Switch)
        {
            Errors.Error.SyntaxError("Switch...Endswitch mismatch");
            return;
        }

        // case statements jump to here
        (x as CommandSwitch).CaseToEndSwitch(this);
    }
}