namespace Tyke.Net.Symbols;

internal interface ILinkable
{
    void ProposeProcedure(Process.ProcessBase procedure);
    int LinkComplete();
}