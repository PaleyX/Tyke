using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Sections;

internal class Statistics
{
    private readonly SectionBase _section;
    private readonly string _type;

    private readonly List<(string description, object value)> _values = [];

    internal Statistics(string type, SectionBase section)
    {
        _type = type;
        _section = section;        
    }

    internal void Add(string text, object value)
    {
        _values.Add((text + ":", value));
    }

    internal void Report()
    {
        Console.WriteLine();
        Console.WriteLine($"{_type} - {_section.Name}");

        var length = _values.Max(v => v.description.Length);

        foreach (var value in _values)
        {
            Console.WriteLine($"{value.description.PadRight(length)} {value.value}");
        }
    }
}