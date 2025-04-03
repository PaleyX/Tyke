using System.Collections.Generic;
using Tyke.Net.Process;

namespace Tyke.Net.Symbols;

internal static class ProcessStack
{
    private static readonly Stack<CommandBase> _stack = new();

    internal static void Push(CommandBase command)
    {
        _stack.Push(command);
    }

    internal static CommandBase Pop()
    {
        if (_stack.Count == 0)
            return null;

        return _stack.Pop();
    }

    internal static CommandBase Peek()
    {
        if (_stack.Count == 0)
            return null;

        return _stack.Peek();
    }
}