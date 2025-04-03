namespace Tyke.Net.Process;

internal class CommandClose() : CommandBase(CommandTypes.Close)
{
    private Sections.SectionBase _section;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // close
        stack.VerifyAndPop("close");

        // something which can be closed
        _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
        if (_section == null)
            return;

        if (_section.CanClose() == false)
            Errors.Error.SyntaxError("Section does not support close");
    }

    internal override CommandBase Process()
    {
        _section.Process(Sections.SectionActions.Close);

        return Next;
    }
}