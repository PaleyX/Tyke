using System;

namespace Tyke.Net.Sections;

[AttributeUsage(AttributeTargets.Class)]
internal class SectionAttribute : Attribute
{
    internal SectionAttribute(string name)
    {
        Name = name;
    }

    internal string Name { get; }
}
