namespace Tyke.Net.Process
{
    internal class CommandDo() : CommandBase(CommandTypes.Do)
    {
        private Sections.SectionBase _section;

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // 2 tokens
            if (!stack.VerifyCount(2))
                return;

            // do
            stack.VerifyAndPop("do");

            // something which can be done
            _section = Symbols.SymbolTable.GetSymbol<Sections.SectionBase>(stack.Pop());
            if (_section == null)
                return;

            if (_section.CanDo() == false)
                Errors.Error.SyntaxError("Section does not support do");
        }

        internal override CommandBase Process()
        {
            _section.Process(Sections.SectionActions.Do);

            return Next;
        }
    }
}
