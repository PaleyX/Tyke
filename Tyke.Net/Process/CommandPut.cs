using JetBrains.Annotations;
using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("put")]
internal class CommandPut() : CommandBase(CommandTypes.Put)
{
    private IActionPut _putable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // 2 tokens
        if (!stack.VerifyCount(2))
            return;

        // put
        stack.VerifyAndPop("put");

        // something which can be put
        _putable = Symbols.SymbolTable.GetActionable<IActionPut>(stack.Pop());
    }

    internal override CommandBase Process()
    {
        _putable.Put();

        return Next;
    }
}