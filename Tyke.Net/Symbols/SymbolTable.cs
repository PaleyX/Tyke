using System;
using System.Collections.Generic;
using System.Linq;
using Tyke.Net.Sections;

namespace Tyke.Net.Symbols;

internal static class SymbolTable
{
    private static readonly Dictionary<string, SymbolBase> _symbols = new();

    // a test hook - used to clear the global symbol table
    // - a bit hacky but needed unless I make the symbol table
    //   an object that is passed around
    internal static void Clear()
    {
        _symbols.Clear();
    }

    internal static T GetActionable<T>(string name) where T: class, IAction
    {
        var section = GetSymbol<SymbolBase>(name);

        if (section is T action)
        {
            return action;
        }

        Errors.Error.ReportError("Section does not support action");

        return null;
    }

    internal static T GetSymbol<T>(string name, bool reportError = true) where T: SymbolBase
    {
        if (!_symbols.TryGetValue(name, out var symbol))
        {
            if(reportError)
                Errors.Error.SyntaxError("Unknown symbol: {0}", name);
            return null;
        }

        if (symbol is T symbolBase)
            return symbolBase;
            
        if(reportError)
            Errors.Error.SyntaxError("Invalid type: {0}", name);

        return null;
    }

    internal static void AddSymbol(string name, SymbolBase symbolBase)
    {
        if (string.IsNullOrEmpty(name))
        {
            Errors.Error.ReportError("Zero length symbol name");
            return;
        }

        // valid name - starts with alphabetic
        if (!char.IsLetter(name[0]))
        {
            Errors.Error.ReportError("Invalid symbol name, must start with a letter");
            return;
        }

        // must be all letters, underscores, hyphens, and numbers
        foreach (var c in name)
        {
            if (char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '-')
                continue; // great

            Errors.Error.ReportError("Invalid character in symbol name: " + c);
            return;
        }

        // add symbol
        symbolBase.Name = name;
        AddSymbol(symbolBase);
    }

    internal static IEnumerable<SectionBase> GetSections()
    {
        return _symbols.Where(s => s.Value is SectionBase).Select(s => s.Value).Cast<SectionBase>();
    }

    internal static IEnumerable<Process.ProcessBase> GetProcedures()
    {
        return _symbols.Where(s => s.Value is Process.ProcessBase).Select(s => s.Value).Cast<Process.ProcessBase>();
    }

    private static void AddSymbol(SymbolBase symbolBase)
    {
        if (string.IsNullOrEmpty(symbolBase.Name))
        {
            Errors.Error.ReportError("Empty symbol name");
            return;
        }

        if (!_symbols.TryAdd(symbolBase.Name, symbolBase))
        {
            Errors.Error.ReportError("Duplicate symbol name: " + symbolBase.Name);
            return;
        }
    }
}