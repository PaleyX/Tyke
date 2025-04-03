using System;
using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Symbols;

internal static class SymbolTable
{
    private static readonly Dictionary<string, SymbolBase> _symbols = new();

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
        if (!Char.IsLetter(name[0]))
        {
            Errors.Error.ReportError("Invalid symbol name, must start with a letter");
            return;
        }

        // must be all letters, underscores, hyphens, and numbers
        foreach (char c in name)
        {
            if (Char.IsLetter(c) || Char.IsDigit(c) || c == '_' || c == '-')
                continue; // great

            Errors.Error.ReportError("Invalid character in symbol name: " + c);
            return;
        }

        // add symbol
        symbolBase.Name = name;
        AddSymbol(symbolBase);
    }

    internal static IEnumerable<Sections.SectionBase> GetSections()
    {
        return _symbols.Where(s => s.Value is Sections.SectionBase).Select(s => s.Value).Cast<Sections.SectionBase>();
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