using JetBrains.Annotations;
using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("close")]
internal class CommandClose : CommandBase
{
    private IActionClose _closable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // close
        stack.VerifyAndPop("close");

        // something which can be closed
        _closable = Symbols.SymbolTable.GetActionable<IActionClose>(stack.Pop());
    }

    internal override CommandBase Process()
    {
        _closable.Close();

        return Next;
    }
}