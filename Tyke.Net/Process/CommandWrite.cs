namespace Tyke.Net.Process
{
    internal class CommandWrite() : CommandBase(CommandTypes.Write)
    {
        private Sections.SectionBase _section;

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // only 2
            if (!stack.VerifyCount(2))
                return;

            // write
            stack.VerifyAndPop("write");

            // something which can be written
            _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());

            if (_section.CanWrite() == false)
                Errors.Error.SyntaxError("Section does not support write");
        }

        internal override CommandBase Process()
        {
            _section.Process(Sections.SectionActions.Write);

            return Next;
        }
    }
}
