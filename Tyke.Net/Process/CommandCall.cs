using JetBrains.Annotations;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("call")]
internal class CommandCall : CommandBase, Symbols.ILinkable
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

    public void ProposeProcedure(ProcessBase procedure)
    {
        if (procedure.Name == _procedureName)
            _procedure = procedure;
    }

    public int LinkComplete()
    {
        if (_procedure is null)
            Errors.Error.ReportError("unable to link to " + _procedureName);

        return _procedure == null ? 1 : 0;
    }
}