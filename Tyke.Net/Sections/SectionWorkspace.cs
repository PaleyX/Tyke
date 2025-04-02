using System;


namespace Tyke.Net.Sections
{
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

        internal override void Process(SectionActions action)
        {
            throw new ApplicationException("Cannot process workspace section");
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
}
