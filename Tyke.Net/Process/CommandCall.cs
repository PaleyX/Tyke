namespace Tyke.Net.Process;

internal class CommandCall() : CommandBase(CommandTypes.Call), Symbols.ILinkable
{
    private ProcessBase _procedure;
    private string _procedureName;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // 2 words
        if (!stack.VerifyCount(2))
            return;

        // call
        stack.VerifyAndPop("call");

        // procedure name
        _procedureName = stack.Pop();
    }

    internal override CommandBase Process()
    {
        _procedure.Run();

        return Next;
    }

    #region ILinakable
    public void ProposeProcedure(ProcessBase procedure)
    {
        if (procedure.Name == _procedureName)
            _procedure = procedure;
    }

    public int LinkComplete()
    {
        if (_procedure == null)
            Errors.Error.ReportError("unable to link to " + _procedureName);

        return _procedure == null ? 1 : 0;
    }
    #endregion ILinkable
}