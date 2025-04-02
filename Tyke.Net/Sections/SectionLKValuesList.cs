using System;
using System.Collections.Generic;

namespace Tyke.Net.Sections
{
    internal class SectionLkValuesList : SectionLkValuesBase, ISectionBlock
    {
        private readonly Dictionary<string, string> _values = new();

        internal SectionLkValuesList()
            : base(SectionTypes.LkValuesList)
        {
            LinesParser = new SectionLines
            (
                this,
                new SectionLine("name", ParseName)
            );
        }

        internal override void PostCompile()
        {
            if (_values == null || _values.Count == 0)
                Errors.Error.ReportError("No values declared");
        }

        internal override void Process(SectionActions action)
        {
            throw new ApplicationException("Cannot process listknife values list section");
        }

        internal override string GetDescription(string value)
        {
            value = Normalise(value);

            return _values.GetValueOrDefault(value, value);
        }

        #region Parsing
        private void ParseName(string value)
        {
            Symbols.SymbolTable.AddSymbol(value, this);
        }
        #endregion Parsing


        #region ISectionBlock
        public void ProcessBlockLine(string line)
        {
            Tuple<string, string> pair = Tools.StringTools.SplitOnEquals(line);

            // quoted string
            string value = Tools.StringTools.GetQuotedString(pair.Item1);
            Tools.StringTools.ValidateNonEmpty(value);

            // quoted string
            string description = Tools.StringTools.GetQuotedString(pair.Item2);
            Tools.StringTools.ValidateNonEmpty(description);

            if(string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(description))
                return;


            value = Normalise(value);               

            if (!_values.TryAdd(value, description))
            {
                Errors.Error.ReportError("Value already exists in values list");
                return;
            }
        }
        #endregion ISectionBlock

        private static string Normalise(string value) => value.ToLower();
    }
}
