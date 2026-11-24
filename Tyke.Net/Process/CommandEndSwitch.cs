using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("endswitch")]
internal class CommandEndSwitch : CommandBase
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyCount(1);
        stack.VerifyAndPop("endswitch");

        // pop process stack
        var x = Symbols.ProcessStack.Pop();
        if (x is not CommandSwitch @switch)
        {
            Errors.Error.SyntaxError("Switch...Endswitch mismatch");
            return;
        }

        // case statements jump to here
        @switch.CaseToEndSwitch(this);
    }
}