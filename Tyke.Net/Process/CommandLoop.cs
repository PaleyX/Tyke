using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("loop")]
internal class CommandLoop : CommandBase
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 1 word
        stack.VerifyCount(1);

        // just one word - loop 
        stack.VerifyAndPop("loop");

        // push onto stack
        Symbols.ProcessStack.Push(this);
    }
}