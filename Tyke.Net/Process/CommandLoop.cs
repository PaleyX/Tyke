namespace Tyke.Net.Process
{
    internal class CommandLoop() : CommandBase(CommandTypes.Loop)
    {
        internal override void ParseCommand(Parser.Tokeniser stack)
        {
	        // only 1 word
	        stack.VerifyCount(1);

	        // just one word - loop 
	        stack.VerifyAndPop("loop");

	        // push onto stack
	        Symbols.ProcessStack.Push(this);
        }
    }
}
