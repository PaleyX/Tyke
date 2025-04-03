using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Parser;

internal class TykeLine
{
    private readonly Stack<char> _chars;

    internal TykeLine(string line)
    {
        _chars = new Stack<char>(line.ToCharArray().Reverse());
    }
        
    internal bool Eol()
    {
        return !_chars.Any();
    }

    internal char Pop()
    {
        return _chars.Pop();
    }

    internal char Peek()
    {
        return _chars.Peek();
    }
}