using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[Command("read")]
internal class CommandRead() : CommandBase(CommandTypes.Read)
{
    private IActionRead _readable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // read
        stack.VerifyAndPop("read");

        // something which can be read
        _readable = Symbols.SymbolTable.GetActionable<IActionRead>(stack.Pop());

        if (!_readable.CanRead)
        {
            Errors.Error.ReportError("Read action incompatible with access type");
        }
    }

    internal override CommandBase Process()
    {
        _readable.Read();

        return Next;
    }
}