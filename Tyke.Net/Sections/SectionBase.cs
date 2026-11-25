using System;
using Tyke.Net.Symbols;

namespace Tyke.Net.Sections;

internal abstract class SectionBase : SymbolBase
{
    protected SectionLines LinesParser;
        
    internal SectionBase(SectionTypes sectionType)
    {
        SectionType = sectionType;
    }

    internal SectionTypes SectionType { get; }

    internal abstract void ReportStatistics();

    internal void Compile()
    {
        if (LinesParser == null)
            throw new NullReferenceException("LinesParser");

        LinesParser.Parse();
    }

    internal virtual void PostCompile() { }
    internal virtual void PreProcess() { }
    internal virtual void PostProcess() { }
}