using System;

namespace Tyke.Net.Sections;

internal class SectionLine
{
    private readonly Action<string> _callback;

    internal SectionLine(string lhs, Action<string> callback, bool required = true, bool oneOnly = true)
    {
        this.Lhs = lhs;
        _callback = callback;
        Required = required;
        OneOnly = oneOnly;
    }

    internal string Lhs { get; private set; }

    internal void Process(string value)
    {
        _callback(value);
        Processed = true;
    }

    internal bool Processed { get; private set; }
    internal bool Required { get; private set; }
    internal bool OneOnly { get; private set; }
}