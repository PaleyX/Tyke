namespace Tyke.Net.Process;

internal abstract class CommandBase
{
    internal abstract void ParseCommand(Parser.Tokeniser stack);
        
    internal virtual CommandBase Process() 
    {
        return Next;
    }

    internal CommandBase Next {get; set;}
}