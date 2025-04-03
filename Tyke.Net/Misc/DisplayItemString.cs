namespace Tyke.Net.Misc;

internal class DisplayItemString : DisplayItemBase
{
    private readonly string _text;

    internal DisplayItemString(string text)
    {
        _text = text;
    }

    internal override string Value()
    {
        return _text;
    }
}