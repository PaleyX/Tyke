namespace Tyke.Net.Process;

internal class CommandCast() : CommandBase(CommandTypes.Cast)
{
    private Data.DatafieldBase _source;
    private Data.DatafieldBase _object;

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // must be 4
        if (!stack.VerifyCount(4))
            return;

        // cast
        stack.VerifyAndPop("cast");

        // datafield
        _source = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());

        // into
        stack.VerifyAndPop("into");

        // datafield
        _object = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());

        // test
        if(_source == null || _object == null)
        {
            Errors.Error.SyntaxError(Errors.StdErrors.ExpectedDatafield);
            return;
        }

        // can cast
        if (!_source.CanCast(_object))
            Errors.Error.SyntaxError("Invalid casting operands");
    }

    internal override CommandBase Process()
    {
        _source.Cast(_object);

        return Next;
    }
}