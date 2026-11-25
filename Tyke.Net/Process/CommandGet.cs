using JetBrains.Annotations;
using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("get")]
internal class CommandGet : CommandBase
{
    private IActionGet _getable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // 2 tokens
        if (!stack.VerifyCount(2))
            return;

        // get
        stack.VerifyAndPop("get");

        // something which can be put
        _getable = Symbols.SymbolTable.GetActionable<IActionGet>(stack.Pop());
    }

    internal override CommandBase Process()
    {
        _getable.Get();

        return Next;
    }
}