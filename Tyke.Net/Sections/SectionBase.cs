using System;
using Tyke.Net.Symbols;

namespace Tyke.Net.Sections
{
    internal abstract class SectionBase : SymbolBase
    {
        protected SectionLines LinesParser;
        
        internal SectionBase(SectionTypes sectionType)
        {
            SectionType = sectionType;
        }

        internal SectionTypes SectionType { get; }

        internal abstract void Process(SectionActions action);
        internal abstract void ReportStatistics();

        internal virtual void Compile()
        {
            if (LinesParser == null)
                throw new NullReferenceException("LinesParser");

            LinesParser.Parse();
        }

        internal virtual void PostCompile() { }
        internal virtual void PreProcess() { }
        internal virtual void PostProcess() { }

        internal virtual bool CanPut() { return false; }
        internal virtual bool CanGet() { return false; }
        internal virtual bool CanDo() { return false; }
        internal virtual bool CanClear() { return false; }

        internal virtual bool CanOpen() { return false; }
        internal virtual bool CanRead() { return false; }
        internal virtual bool CanWrite() { return false; }
        internal virtual bool CanClose() { return false; }
    }
}
