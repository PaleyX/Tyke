namespace Tyke.Net.Process;

internal class CommandElse() : CommandBase(CommandTypes.Else)
{
    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // only 1 word and must be "else"
        stack.VerifyCount(1);
        stack.VerifyAndPop("else");

        Symbols.ProcessStack.Push(this);
    }
}