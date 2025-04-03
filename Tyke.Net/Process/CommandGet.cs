namespace Tyke.Net.Process;

internal class CommandGet() : CommandBase(CommandTypes.Get)
{
    private Sections.SectionBase _section;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // 2 tokens
        if (!stack.VerifyCount(2))
            return;

        // get
        stack.VerifyAndPop("get");

        // something which can be put
        _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
        if (_section == null)
            return;

        if (_section.CanGet() == false)
            Errors.Error.SyntaxError("Section does not support get");
    }

    internal override CommandBase Process()
    {
        _section.Process(Sections.SectionActions.Get);

        return Next;
    }
}