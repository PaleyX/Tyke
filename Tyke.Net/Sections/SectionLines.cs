using System.Collections.Generic;
using System.Linq;

namespace Tyke.Net.Sections;

internal class SectionLines
{
    private readonly SectionBase _section;
    private readonly List<SectionLine> _lines;

    internal SectionLines(SectionBase section, params SectionLine[] lines)
    {
        _section = section;
        _lines = [.. lines];
    }

    internal void Parse()
    {
        while (true)
        {
            string line = Parser.TykeFile.GetLine();

            if(line.ToLowerInvariant() == "end")
                break;

            // start of a begin...end block?
            if (line.ToLowerInvariant() == "begin")
            {
                ParseBlock();
                continue;
            }

            //TODO: make sure line doesn't end with '=' so "Name="
            
            var pair = Tools.StringTools.SplitOnEquals(line);
            if (pair == default)
                continue;

            var lhs = pair.Item1;
            var rhs = pair.Item2;

            //Console.WriteLine("[{0}]", lhs);
            //Console.WriteLine("[{0}]", rhs);

            // look for lhs in list of lines
            var found = _lines.FirstOrDefault(x => string.Compare(x.Lhs, lhs, true) == 0);
            if (found == null)
            {
                Errors.Error.ReportError("Unknown line");
                continue;
            }

            if (found.Processed && found.OneOnly)
            {
                Errors.Error.ReportError("duplicate line");
                continue;
            }

            // process
            found.Process(rhs);
        }

        // do we have everything?
        foreach (var line in _lines)
        {
            if (line.Required && !line.Processed)
                Errors.Error.ReportError("Line missing: " + line.Lhs);
        }
    }

    private void ParseBlock()
    {
        if (_section is not ISectionBlock block)
        {
            Errors.Error.SyntaxError("Section does not support begin...end block");
            return;
        }

        while (true)
        {
            var line = Parser.TykeFile.GetLine();

            if (line.ToLowerInvariant() == "end")
                break;

            block.ProcessBlockLine(line);
        }
    }
}