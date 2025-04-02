namespace Tyke.Net.Process
{
    internal class ProcessProcedure : ProcessBase
    {
        internal ProcessProcedure(string name)
        {
            Symbols.SymbolTable.AddSymbol(name, this);
        }
    }
}
