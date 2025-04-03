using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Tyke.Net.Sections;

internal class SectionLkFilter : SectionBase, ISectionBlock
{
    private string _description;
    private SectionLkDatabase _database;
    private readonly List<Data.DatafieldBase> _fields = [];
    private byte[] _buffer;
    private int _filterId = int.MinValue;

    private uint _put;
    private uint _added;

    internal SectionLkFilter()
        : base(SectionTypes.LkFilter)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("description", ParseDescription),
            new SectionLine("database", ParseDatabase)
        );
    }

    internal string Description => _description;

    internal override void Process(SectionActions action)
    {
        Debug.Assert(action == SectionActions.Put);

        ++_put;

        // first
        if (_put == 1)
        {
            CopyBuffer();
            _database.AddFilterFlag(_filterId);
            ++_added;
            return;
        }

        // compare with fields
        int offset = 0;
        int result = 0;
        foreach (var datafield in _fields)
        {
            var x = datafield.ToString();
            result = datafield.CompareWithBuffer(_buffer, offset);
            if (result != 0)
                break;

            offset += datafield.Length;
        }

        if(result != 0)
        {
            CopyBuffer();
            _database.AddFilterFlag(_filterId);
            ++_added;
        }
    }

    internal override void ReportStatistics()
    {
        var stats = new Statistics("listknife filter", this);

        stats.Add("Put", _put);
        stats.Add("Added", _added);

        stats.Report();
    }

    internal override bool CanPut()
    {
        return true;
    }

    internal override void PostCompile()
    {
        if (_fields.Count == 0)
        {
            Errors.Error.ReportError("no filter fields declared");
            return;
        }

        // create buffer of length of all fields
        _buffer = new byte[_fields.Sum(f => f.Length)];
    }

    private void CopyBuffer()
    {
        Debug.Assert(_buffer != null);

        int i = 0;

        foreach (var datafield in _fields)
        {
            datafield.CopyToBuffer(_buffer, i);
            i += datafield.Length;
        }
    }

    #region Parsing
    private void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    private void ParseDescription(string value)
    {
        _description = Tools.StringTools.GetQuotedString(value);
        Tools.StringTools.ValidateNonEmpty(value);
    }

    private void ParseDatabase(string value)
    {
        _database = Symbols.SymbolTable.GetSymbol<SectionLkDatabase>(value);
        if (_database == null)
            Errors.Error.ReportError("Expected ListKnife database");
        else
            _filterId = _database.RegisterFilter(this);
    }

    public void ProcessBlockLine(string line)
    {

        // one datafield
        Parser.Tokeniser stack = new Parser.Tokeniser(line);

        // just one
        if (!stack.VerifyCount(1))
            return;

        // must be a datafield
        Data.DatafieldBase datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());
        if (datafield != null)
            _fields.Add(datafield);
    }
    #endregion Parsing

}