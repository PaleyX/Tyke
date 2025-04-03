namespace Tyke.Net.Process;

internal class CommandOpen() : CommandBase(CommandTypes.Open)
{
    private Sections.SectionBase _section;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 2
        if (!stack.VerifyCount(2))
            return;

        // open
        stack.VerifyAndPop("open");

        // something which can be opened
        _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
        if (_section == null)
            return;

        if(_section.CanOpen() == false)
            Errors.Error.SyntaxError("Section does not support open");
    }

    internal override CommandBase Process()
    {
        _section.Process(Sections.SectionActions.Open);

        return Next;
    }
}