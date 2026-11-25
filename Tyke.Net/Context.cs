using System.Collections.Generic;
using Tyke.Net.Symbols;

namespace Tyke.Net
{
    internal class Context()
    {
        internal Dictionary<string, SymbolBase> Symbols { get; } = new();

        internal bool ThrowOnError { get; set; } = false;
    }
}
