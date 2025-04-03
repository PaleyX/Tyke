using System;
using System.IO;

namespace Tyke.Net.Parser;

internal static class TykeFile
{
    private static TextReader _reader;
    private static int _lineNumber;

    internal static void Open(string pathname)
    {
        _reader = new StreamReader(pathname);
        IsOpen = true;
    }

    internal static string GetLine()
    {
        string line;
        bool inSkip = false;

        while (true)
        {
            line = _reader.ReadLine();

            if (line == null)
                return null;

            ++_lineNumber;

            // echo to screen
            if (inSkip)
                Console.WriteLine("(S){0}:{1}", _lineNumber, line);
            else
                Console.WriteLine("{0}:{1}", _lineNumber, line);

            // #skip
            if (line.Trim().ToLower().StartsWith("#skip"))
            {
                inSkip = true;
                continue;
            }

            // #endskip
            if (line.Trim().ToLower().StartsWith("#endskip"))
            {
                if (inSkip)
                    inSkip = false;
                else
                    Errors.Error.SyntaxError("Mismatched #skip...#endskip");
                continue;
            }

            if (inSkip)
                continue;

            // remove leading whitespace
            line = line.Trim();

            // skip single line comments
            if (line.StartsWith("^"))
                continue;

            // skip whitespace lines
            if (line.Length == 0)
                continue;

            // inline comments?
            bool inQuote = false;
            for (int i = 0; i < line.Length; ++i)
            {

                char c = line[i];

                if(c == '"')
                {
                    inQuote = !inQuote;
                    continue;
                }

                if(c == '^')
                {
                    if(!inQuote)
                    {
                        line = line.Substring(0, i);
                        break;
                    }
                }
            }

            // looks good, go for it
            break;
        }

        return line;
    }

    internal static void Close()
    {
        if (IsOpen)
        {
            _reader.Close();
            IsOpen = false;
        }
    }

    internal static bool IsOpen { get; private set; }
}