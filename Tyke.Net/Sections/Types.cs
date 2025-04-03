namespace Tyke.Net.Sections;

internal enum SectionTypes
{
    Workspace,
    File,
    SqlDatabase,
    SqlReader,
    LkDatabase,
    LkAttribute,
    LkFilter,
    LkValuesList,
    LkValuesProcedure,
    Tokenizer
}

internal enum SectionActions
{
    Undefined,
    Put,
    Get,
    Do,
    Clear,
    Open,
    Read,
    Write,
    Close
}