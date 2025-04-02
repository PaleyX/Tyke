namespace Tyke.Net.Process
{
    internal class CommandPut() : CommandBase(CommandTypes.Put)
    {
        private Sections.SectionBase _section;

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // 2 tokens
            if (!stack.VerifyCount(2))
                return;

            // put
            stack.VerifyAndPop("put");

            // something which can be put
            _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
            if (_section == null)
                return;

            if (_section.CanPut() == false)
                Errors.Error.SyntaxError("Section does not support put");
        }

        internal override CommandBase Process()
        {
            _section.Process(Sections.SectionActions.Put);

            return Next;
        }
    }
}
