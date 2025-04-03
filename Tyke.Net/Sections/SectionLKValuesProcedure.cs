namespace Tyke.Net.Sections;

internal class SectionLkValuesProcedure : SectionLkValuesBase, Symbols.ILinkable
{
    private string _procedureName;
    private Process.ProcessBase _procedure;

    private Data.DatafieldBase _value;
    private Data.DatafieldAlpha _description;

    internal SectionLkValuesProcedure()
        : base(SectionTypes.LkValuesProcedure)
    {
        LinesParser = new SectionLines
        (
            this,
            new SectionLine("name", ParseName),
            new SectionLine("procedure", ParseProcedure),
            new SectionLine("value field", ParseValueField),
            new SectionLine("description field", ParseDescriptionField)
        );
    }

    internal override string GetDescription(string value)
    {
        // space fill description
        _description.Set(string.Empty);

        // set value fielf to value
        _value.Set(value);

        // call procedure
        _procedure.Run();

        return _description.ToString();
    }

    internal override void Process(SectionActions action)
    {
        // nowt
    }

    #region Parsing
    private void ParseName(string value)
    {
        Symbols.SymbolTable.AddSymbol(value, this);
    }

    private void ParseProcedure(string value)
    {
        _procedureName = value;
    }

    private void ParseValueField(string value)
    {
        _value = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(value);
        if (_value == null)
            Errors.Error.SyntaxError(Errors.StdErrors.ExpectedDatafield);
    }

    private void ParseDescriptionField(string value)
    {
        _description = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(value);
        if (_description == null)
            Errors.Error.SyntaxError(Errors.StdErrors.ExpectedAlphaDatafield);
    }
    #endregion Parsing

    #region ILinkable
    public void ProposeProcedure(Process.ProcessBase procedure)
    {
        if (_procedureName == procedure.Name)
            _procedure = procedure;
    }

    public int LinkComplete()
    {
        if (_procedure == null)
            Errors.Error.ReportError("unable to link to " + _procedureName);

        return _procedure == null ? 1 : 0;
    }
    #endregion ILinkable
}