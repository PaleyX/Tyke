namespace Tyke.Net.Process;

internal class CommandInc() : CommandBase(CommandTypes.Inc)
{
    private Data.DatafieldBinary _field;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        if (!stack.VerifyCount(2))
            return;

        stack.VerifyAndPop("inc");

        // next must be numeric variable
        var token = stack.Pop();

        _field = Symbols.SymbolTable.GetSymbol<Data.DatafieldBinary>(token);
        if (_field == null)
            return;
    }

    internal override CommandBase Process()
    {
        _field.Increment();

        return base.Process();
    }
}