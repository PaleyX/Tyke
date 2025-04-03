namespace Tyke.Net.Sections;

internal class SectionLkAttribute: SectionBase
{
    private string _description;
    private Data.DatafieldBase _field;
    private SectionLkDatabase _database;
    private SectionLkValuesBase _values;
    private string _group;

    private int _attributeId;
    private int _put;
    private int _nonEmpty;

    internal SectionLkAttribute() 
        : base(SectionTypes.LkAttribute)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("values", ParseValues, false),
            new SectionLine("description", ParseDescription),
            new SectionLine("value field", ParseValueField),
            new SectionLine("database", ParseDatabase),
            new SectionLine("group", ParseGroup, false)
        );
    }

    internal string Description => _description;
    internal string Group => _group;

    internal string TypeCode
    {
        get
        {
            if (_field is Data.DatafieldAlpha)
                return "A";

            if (_field is Data.DatafieldBinary)
                return "N";

            throw new TykeRunTimeException("Unknown field type", Name);
        }
    }

    internal string GetValueDescription(string value)
    {
        if (_values != null)
            return _values.GetDescription(value);

        return value;
    }

    internal override bool CanPut() 
    { 
        return true; 
    }

    internal override void PostCompile()
    {
        if (_database == null)
            return;

        _attributeId = _database.RegisterAttribute(this);
    }

    internal override void Process(SectionActions action)
    {
        if(action != SectionActions.Put)
            throw new TykeRunTimeException("Unsupported action in LKAttribute", Name);

        ++_put;

        // get value as string
        string value = _field.ToString();

        if (string.IsNullOrWhiteSpace(value))
            return;

        ++_nonEmpty;

        // put in listknife database section
        _database.AddAttributeValue(_attributeId, value);
    }

    internal override void ReportStatistics()
    {
        //TODO: Statistics
    }

    #region Parsing
    private void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    private void ParseValues(string value)
    {
        _values = Symbols.SymbolTable.GetSymbol<SectionLkValuesBase>(value);
        if (_values == null)
            Errors.Error.ReportError("Expected ListKnife values section");
    }

    private void ParseDescription(string value)
    {
        _description = Tools.StringTools.GetQuotedString(value);
        Tools.StringTools.ValidateNonEmpty(value);
    }

    private void ParseValueField(string value)
    {
        _field = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(value);
    }

    private void ParseDatabase(string value)
    {
        _database = Symbols.SymbolTable.GetSymbol<SectionLkDatabase>(value);
        if (_database == null)
            Errors.Error.ReportError("Expected ListKnife database");
    }

    private void ParseGroup(string value)
    {
        _group = Tools.StringTools.GetQuotedString(value);
        Tools.StringTools.ValidateNonEmpty(_group);
    }
    #endregion Parsing
}