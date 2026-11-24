using JetBrains.Annotations;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Tyke.Net.Sections;

[UsedImplicitly]
[Section("sqlreader")]
internal class SectionSqlReader : SectionBase, ISectionBlock, IActionOpen, IActionRead, IActionClose
{
    private SectionSqlDatabase _database;
    private Data.OperandAlpha _query;
    private readonly List<Data.DatabaseColumn> _columns = [];
    private readonly List<Tuple<Data.DatafieldBase, string>> _parameters = [];

    private int _open;
    private int _read;
    private int _close;

    private SqlCommand _command;
    private SqlDataReader _reader;

    internal SectionSqlReader()
        : base(SectionTypes.SqlReader)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("database", ParseDatabase),
            new SectionLine("query", ParseQuery),
            new SectionLine("parameter", ParseParameter, false, false)
        );
    }

    public void Open()
    {
        if (_reader != null)
            throw new TykeRunTimeException("Attempt to open already open SqlReader", Name);

        try
        {
            _command = new SqlCommand(_query.ToString(), _database.GetConnection());
                
            if (_parameters.Count > 0)
                AddParameters();

            ColumnMapping();
            _reader = _command.ExecuteReader();
        }
        catch (Exception e)
        {
            throw new TykeRunTimeException("Cannot open SqlReader: " + e.Message, Name);
        }

        ++_open;
    }

    public void Read()
    {
        try
        {
            bool result = _reader.Read();
            if (result)
            {
                ++_read;
                SetDatafields();
            }

            Expressions.Indicators.IsEof = !result;
        }
        catch (Exception e)
        {
            throw new TykeRunTimeException("Error reading SqlReader: " + e.Message, Name);
        }
    }

    public bool CanRead => true;

    public void Close()
    {
        if (_reader == null)
            throw new TykeRunTimeException("Attempt to close unopen SqlReader", Name);

        try
        {
            _reader.Close();
            _database.ReleaseConnection();
            _reader = null;
        }
        catch (Exception e)
        {
            throw new TykeRunTimeException("Cannot close SqlReader: " + e.Message, Name);
        }

        ++_close;
    }

    internal override void ReportStatistics()
    {
        var stats = new Statistics("SqlReader", this);

        stats.Add("Opened", _open);
        stats.Add("Read", _read);
        stats.Add("Closed", _close);

        stats.Report();
    }
        
    #region Parsing
    protected void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    private void ParseDatabase(string value)
    {
        _database = Symbols.SymbolTable.GetSymbol<SectionSqlDatabase>(value);
    }

    private void ParseQuery(string value)
    {
        _query = new Data.OperandAlpha();
        _query.SetFromToken(value);

    }

    private void ParseParameter(string value)
    {
        // 3 values
        // <datafield> to <parameter name>

        Parser.Tokeniser stack = new Parser.Tokeniser(value);

        // 3 tokens
        if (!stack.VerifyCount(3))
            return;

        // first is datafield
        var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop());

        // to
        stack.VerifyAndPop("to");

        // parameter name - starts with @ 
        string parameter = stack.Pop();

        if (!parameter.StartsWith("@"))
            Errors.Error.SyntaxError("Parameter name must start with '@'");

        _parameters.Add(new Tuple<Data.DatafieldBase, string>(datafield, parameter));

    }
    #endregion Parsing

    #region ISectionBlock
    public void ProcessBlockLine(string line)
    {  
        var pair = Tools.StringTools.SplitOnEquals(line);

        // first is data field
        var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(pair.Item1);

        // second is quoted string
        var columnName = Tools.StringTools.GetQuotedString(pair.Item2);

        if (datafield != null && !string.IsNullOrWhiteSpace(columnName))
            _columns.Add(new Data.DatabaseColumn(datafield, columnName));
    }
    #endregion ISectionBlock

    private void ColumnMapping()
    {
        try
        {
            using var reader = _command.ExecuteReader(CommandBehavior.SchemaOnly);
            var schema = reader.GetSchemaTable();

            foreach (DataRow row in schema.Rows)
            {
                string name = row[0].ToString();
                int ordinal = (int)row[1];
                int size = (int)row[2];
                string type = row[24].ToString();

                // find column
                var map = _columns.FirstOrDefault(c => string.Compare(c.MappedTo, name, true) == 0);
                map?.SetMapping(ordinal, size, type);
            }
        }
        catch (Exception e)
        {
            throw new TykeRunTimeException("Error mapping columns: " + e.Message, Name);
        }

        // any not mapped
        if (_columns.Count(c => !c.IsMapped) > 0)
        {
            Errors.Error.NonTerminatingRuntimeError("Unmapped column(s) found opening SqlReader: {0}", Name);

            foreach (var column in _columns.Where(c => !c.IsMapped))
            {
                Errors.Error.NonTerminatingRuntimeError("No mapping found for {0}", column.MappedTo);
            }

            throw new TykeRunTimeException("Unmapped columns", Name);
        }
    }

    private void SetDatafields()
    {
        foreach (var column in _columns)
        {
            column.Set(_reader);
        }
    }

    private void AddParameters()
    {
        Debug.Assert(_command != null);

        foreach (var parameter in _parameters)
        {
            switch(parameter.Item1.DatafieldType)
            {
                case Data.DatafieldBase.DatafieldTypes.Alpha:
                    _command.Parameters.Add(parameter.Item2, SqlDbType.NVarChar);
                    _command.Parameters[parameter.Item2].Value = (parameter.Item1 as Data.DatafieldAlpha).ToString();
                    break;
                case Data.DatafieldBase.DatafieldTypes.Binary:
                    _command.Parameters.Add(parameter.Item2, SqlDbType.Int);
                    _command.Parameters[parameter.Item2].Value = Convert.ToInt32((parameter.Item1 as Data.DatafieldBinary).GetDWord());
                    break;
                default:
                    throw new TykeRunTimeException("Unknown datafield type", Name);
            }
        }
    }
}