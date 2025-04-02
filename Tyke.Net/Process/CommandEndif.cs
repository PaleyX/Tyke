namespace Tyke.Net.Process
{
    internal class CommandEndif() : CommandBase(CommandTypes.Endif)
    {
        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            bool gotIf = false;
            CommandBase commandElse = null;

            // first and only word must be endif
            stack.VerifyCount(1);
            stack.VerifyAndPop("endif");

            // is there an if
            CommandBase x;

            while ((x = Symbols.ProcessStack.Pop()) != null)
            {
                if (x.CommandType == CommandTypes.If || x.CommandType == CommandTypes.IfNot)
                {
                    var y = x as CommandBranchBase;
                    if (commandElse == null)
                    {
                        // no else so jump to endif
                        y.Jump = this;
                    }
                    else
                    {
                        // else - so jump to NOP after else
                        y.Jump = commandElse.Next;
                        // else goes to endif
                        commandElse.Next = this;
                    }
                    gotIf = true;
                    break;
                }
                if (x.CommandType == CommandTypes.Else)
                {
                    if (commandElse != null)
                        Errors.Error.SyntaxError("Multiple else defined");
                    commandElse = x;
                }
                else
                {
                    Errors.Error.SyntaxError("mismatched if...endif");
                    return;
                }
            }

            if (!gotIf)
                Errors.Error.SyntaxError("mismatched if...endif");
        }
    }
}
