using System;

namespace Tyke.Net.Process;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]

internal class CommandAttribute : Attribute
{
    internal CommandAttribute(string name)
    {
        Name = name;
    }

    internal string Name { get; }
}
