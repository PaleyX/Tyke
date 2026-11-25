using System;
using System.Linq;
using System.Reflection;
using Tyke.Net.Process;
using Tyke.Net.Sections;

namespace Tyke.Net.Tools;

internal static class AttributeTools
{
    internal static SectionBase GetSection(string name)
    {
        var type = typeof(SectionBase).Assembly
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(SectionBase).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.GetCustomAttribute<SectionAttribute>()?.Name == name);

        return type is null
            ? null
            : (SectionBase)Activator.CreateInstance(type, nonPublic: true)!;
    }

    internal static CommandBase GetCommand(string name)
    {
        var type = typeof(CommandBase).Assembly
            .GetTypes()
            .FirstOrDefault(t =>
                typeof(CommandBase).IsAssignableFrom(t) &&
                !t.IsAbstract &&
                t.GetCustomAttributes<CommandAttribute>()
                    .Any(a => a.Name == name));

        return type is null
            ? null
            : (CommandBase)Activator.CreateInstance(type, nonPublic: true)!;
    }
}