namespace Tyke.Net.Sections;

// marker interface - acceptable when used as a generic constraint
internal interface IAction;

internal interface IActionPut : IAction
{
    void Put();
}

internal interface IActionGet : IAction
{
    void Get();
}

internal interface IActionDo : IAction
{
    void Do();
}

internal interface IActionOpen : IAction
{
    void Open();
}

internal interface IActionRead : IAction
{
    void Read();
    bool CanRead { get; }
}

internal interface IActionWrite : IAction
{
    void Write();
    bool CanWrite { get; }
}

internal interface IActionClose : IAction
{
    void Close();
}
