using System.Collections.Generic;

namespace Tyke.Net.Data;

internal class OperandList<T> where T: OperandBase
{
    private readonly List<T> _list = [];

    internal void ParseList(Parser.Tokeniser stack)
    {
        // a list of compatible data separated by commas
        foreach (string token in stack.GetList())
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
        T x = OperandHelpers.AllocateOpFromToken(token) as T;
        if (x == null)
            Errors.Error.SyntaxError(Errors.StdErrors.IncompatibleOperands);
        else
            _list.Add(x);
    }
}