namespace Tyke.Net.Process;

internal class CommandEndLoop() : CommandBase(CommandTypes.EndLoop)
{
    private CommandBase _loopBack;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        bool   gotLoop = false;
        bool   gotUntil = false;

        // just one word - endloop 
        stack.VerifyAndPop("endloop");

        // OK - is there a loop & untils 
        CommandBase x;

        while((x = Symbols.ProcessStack.Pop()) != null)
        {
            if(x.CommandType == CommandTypes.Loop)
            {
                _loopBack = x.Next;	
                gotLoop = true;
                break;
            }
            else
            if(x.CommandType == CommandTypes.Until)
            {
                (x as CommandBranchBase).Jump = Next;

                //static_cast<CProcBranch*>(x) -> cpcJump = cpcNext;
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