using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Sections;

internal class SectionTokenizer : SectionBase
{
    private Data.DatafieldAlpha _datafield;
    private Data.DatafieldAlpha _tokenField;
    //private Data.DatafieldAlpha _SeparatorField;
    private char[] _separators;
    private bool _skipLeadingWhitespace;

    private Stack<string> _data;
    private int _put;
    private int _get;

    internal SectionTokenizer()
        : base(SectionTypes.Tokenizer)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("data field", ParseDataField),
            new SectionLine("token field", ParseTokenField),
            //new SectionLine("separator field", ParseSeparatorField),
            new SectionLine("Separators", ParseSeparators),
            new SectionLine("skip leading white space", ParseSkipLeadingWhitespace, false)
        );
    }

    internal override bool CanPut() { return true; }
    internal override bool CanGet() { return true; }

    internal override void Process(SectionActions action)
    {
        switch (action)
        {
            case SectionActions.Put:
                ProcessPut();
                break;
            case SectionActions.Get:
                ProcessGet();
                break;
            default:
                throw new ApplicationException("SectionTokenizer::Process");
        }
    }

    internal override void ReportStatistics()
    {
        //TODO:
    }

    private void ProcessPut()
    {
        ++_put;
            
        string s = _datafield.ToString();

        if (_skipLeadingWhitespace)
            s = s.TrimStart();

        if (string.IsNullOrWhiteSpace(s))
            _data = null;
        else
            _data = new Stack<string>(s.Split(_separators, StringSplitOptions.RemoveEmptyEntries).Reverse());

        Expressions.Indicators.IsEmpty = false;

    }

    private void ProcessGet()
    {
        ++_get;

        // Clear token field
        _tokenField.Set("");

        if (_data == null || _data.Count == 0)
        {
            Expressions.Indicators.IsEmpty = true;
            return;
        }

        _tokenField.Set(_data.Pop());

        Expressions.Indicators.IsEmpty = false;
    }

    #region Parsing
    private void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    private void ParseDataField(string value)
    {
        _datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(value);
        if (_datafield == null)
            return;
    }

    private void ParseTokenField(string value)
    {
        _tokenField = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(value);
    }

    //private void ParseSeparatorField(string value)
    //{
    //    _SeparatorField = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(value);
    //}

    private void ParseSeparators(string value)
    {
        if (!Tools.StringTools.IsQuotedString(value))
        {
            Errors.Error.SyntaxError(Errors.StdErrors.ExpectedAlphaLiteral);
            return;
        }

        string s = Tools.StringTools.GetQuotedString(value);
        if (Tools.StringTools.ValidateNonEmpty(s))
        {
            _separators = s.ToCharArray();
        }
    }

    private void ParseSkipLeadingWhitespace(string value)
    {
        _skipLeadingWhitespace = Tools.StringTools.GetBoolean(value);
    }
    #endregion Parsing
}