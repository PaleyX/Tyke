using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("endif")]
internal class CommandEndif : CommandBase
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        var gotIf = false;
        CommandBase commandElse = null;

        // first and only word must be endif
        stack.VerifyCount(1);
        stack.VerifyAndPop("endif");

        // is there an if
        while (Symbols.ProcessStack.Pop() is { } x)
        {
            if (x is CommandIf or CommandIfNot)
            {
                var y = (CommandBranchBase)x;
                if (commandElse == null)
                {
                    // no else so jump to endif
                    y.Jump = this;
                }
                else
                {
                    // else - so jump to NOP after else
                    y.Jump = commandElse.Next;
                    // else goes to endif
                    commandElse.Next = this;
                }
                gotIf = true;
                break;
            }
            if (x is CommandElse)
            {
                if (commandElse != null)
                    Errors.Error.SyntaxError("Multiple else defined");
                commandElse = x;
            }
            else
            {
                Errors.Error.SyntaxError("mismatched if...endif");
                return;
            }
        }

        if (!gotIf)
            Errors.Error.SyntaxError("mismatched if...endif");
    }
}