using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("endloop")]
internal class CommandEndLoop : CommandBase
{
    private CommandBase _loopBack;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        var gotLoop = false;
        var gotUntil = false;

        // just one word - endloop 
        stack.VerifyAndPop("endloop");

        // OK - is there a loop & untils 

        while(Symbols.ProcessStack.Pop() is { } x)
        {
            if(x is CommandLoop)
            {
                _loopBack = x.Next;	
                gotLoop = true;
                break;
            }
            else
            if(x is CommandUntil until)
            {
                until.Jump = Next;
                gotUntil = true;
            }
            else
            {
                Errors.Error.SyntaxError("Mismatched loop-endloop pair");
                return;
            }
        }

        if(!gotLoop)
            Errors.Error.SyntaxError("Mismatched loop-endloop pair");

        if(!gotUntil)
            Errors.Error.SyntaxError("No until clauses in loop..endloop");
    }

    internal override CommandBase Process()
    {
        return _loopBack;
    }
}