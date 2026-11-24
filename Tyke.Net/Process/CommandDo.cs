
using JetBrains.Annotations;
using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("do")]
internal class CommandDo() : CommandBase(CommandTypes.Do)
{
    private IActionDo _doable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // 2 tokens
        if (!stack.VerifyCount(2))
            return;

        // do
        stack.VerifyAndPop("do");

        // something which can be done
        _doable = Symbols.SymbolTable.GetActionable<IActionDo>(stack.Pop());
    }

    internal override CommandBase Process()
    {
        _doable.Do();

        return Next;
    }
}