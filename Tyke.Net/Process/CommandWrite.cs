using JetBrains.Annotations;
using Tyke.Net.Sections;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("write")]
internal class CommandWrite() : CommandBase(CommandTypes.Write)
{
    private IActionWrite _writable;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // write
        stack.VerifyAndPop("write");

        // something which can be written
        _writable = Symbols.SymbolTable.GetActionable<IActionWrite>(stack.Pop());

        if (!_writable.CanWrite)
        {
            Errors.Error.ReportError("Read action incompatible with access type");
        }
    }

    internal override CommandBase Process()
    {
        _writable.Write();

        return Next;
    }
}