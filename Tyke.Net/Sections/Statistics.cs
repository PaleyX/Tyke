using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Sections;

internal class Statistics
{
    private readonly SectionBase _section;
    private readonly string _type;

    private readonly List<Tuple<string, object>> _values = [];

    internal Statistics(string type, SectionBase section)
    {
        _type = type;
        _section = section;        
    }

    internal void Add(string text, object value)
    {
        _values.Add(new Tuple<string, object>(text + ":", value));
    }

    internal void Report()
    {
        Console.WriteLine();
        Console.WriteLine("{0} - {1}", _type, _section.Name);

        int length = _values.Max(v => v.Item1.Length);

        foreach (var value in _values)
        {
            Console.WriteLine("{0} {1}", value.Item1.PadRight(length), value.Item2);
        }
    }
}