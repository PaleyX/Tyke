namespace Tyke.Net.Process
{
    internal class CommandNop() : CommandBase(CommandTypes.Nop)
    {
        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // do nowt
        }
    }
}
