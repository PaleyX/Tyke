using JetBrains.Annotations;

namespace Tyke.Net.Sections;

[UsedImplicitly]
[Section("workspace")]
internal class SectionWorkspace : SectionBufferBase
{
    internal SectionWorkspace()
        : base(SectionTypes.Workspace)
    {
        LinesParser = new SectionLines
        (
            this, 
            new SectionLine("name", ParseName),
            new SectionLine("length", ParseLength)
        );
    }

    internal override void ReportStatistics()
    {
        // Nowt
    }

    private void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }
}