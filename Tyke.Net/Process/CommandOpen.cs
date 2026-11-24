using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[Command("open")]
internal class CommandOpen() : CommandBase(CommandTypes.Open)
{
    private IActionOpen _openable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // open
        stack.VerifyAndPop("open");

        // something which can be opened
        _openable = Symbols.SymbolTable.GetActionable<IActionOpen>(stack.Pop());
    }

    internal override CommandBase Process()
    {
        _openable.Open();

        return Next;
    }
}