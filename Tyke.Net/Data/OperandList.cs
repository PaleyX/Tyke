using System.Collections.Generic;

namespace Tyke.Net.Data;

internal class OperandList<T> where T: OperandBase
{
    private readonly List<T> _list = [];

    internal void ParseList(Parser.Tokeniser stack)
    {
        // a list of compatible data separated by commas
        foreach (var token in stack.GetList())
        {
            AddElement(token);
        }
    }

    internal IEnumerable<T> GetList()
    {
        return _list;
    }

    private void AddElement(string token)
    {
        if (OperandHelpers.AllocateOpFromToken(token) is not T x)
            Errors.Error.SyntaxError(Errors.StdErrors.IncompatibleOperands);
        else
            _list.Add(x);
    }
}