namespace Tyke.Net.Process
{
    internal class CommandRead() : CommandBase(CommandTypes.Read)
    {
        private Sections.SectionBase _section;

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // only 2
            if (!stack.VerifyCount(2))
                return;

            // read
            stack.VerifyAndPop("read");

            // something which can be read
            _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
            if (_section == null)
                return;

            if (_section.CanRead() == false)
                Errors.Error.SyntaxError("Section does not support read");
        }

        internal override CommandBase Process()
        {
            _section.Process(Sections.SectionActions.Read);

            return Next;
        }
    }
}
