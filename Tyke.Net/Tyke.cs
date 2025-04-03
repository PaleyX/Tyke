using System;
using System.Diagnostics;

namespace Tyke.Net;

internal static class Tyke
{
    private static Process.Process _process;

    internal static int Run(string pathname)
    {
        var result = 0;

        // open control file
        try
        {
            Parser.TykeFile.Open(pathname);

            for (; ; )
            {
                var line = Parser.TykeFile.GetLine();
                if (line == null)
                    break;

                // special sections
                if (line == "datafields")
                {
                    Data.Datafields.Compile();
                    continue;
                }

                if (line == "process")
                {
                    if (_process != null)
                    {
                        Errors.Error.ReportError("Only one process section allowed");
                        break;
                    }

                    _process = new Process.Process();
                    _process.Compile();

                    continue;
                }

                // named stuff
                var stack = new Parser.Tokeniser(line);
                switch (stack.Peek())
                {
                    case "procedure":
                        ParseProcedure(stack);
                        continue;
                }

                // has to be a section
                ParseSection(line);
            }
        }
        catch (Exception e)
        {
            Errors.Error.ReportError("Error running Tyke.Net: " + e.Message);
        }
        finally
        {
            Parser.TykeFile.Close();
        }

        if (Errors.Error.HasErrors)
            Console.WriteLine("{0} Errors found", Errors.Error.ErrorCount);
        else
        {
            Console.WriteLine("No errors found during compile");

            if (Symbols.Linker.RunLinker(_process))
            {
                RunMain();
            }
        }

        return result;
    }

    private static void ParseSection(string line)
    {
        // normal sections
        Sections.SectionBase section = null;

        switch (line)
        {
            case "workspace":
                section = new Sections.SectionWorkspace();
                break;
            case "file":
                section = new Sections.SectionFile();
                break;
            case "sqldatabase":
                section = new Sections.SectionSqlDatabase();
                break;
            case "sqlreader":
                section = new Sections.SectionSqlReader();
                break;
            case "listknife database":
                section = new Sections.SectionLkDatabase();
                break;
            case "listknife attribute":
                section = new Sections.SectionLkAttribute();
                break;
            case "listknife filter":
                section = new Sections.SectionLkFilter();
                break;
            case "listknife values list":
                section = new Sections.SectionLkValuesList();
                break;
            case "listknife values procedure":
                section = new Sections.SectionLkValuesProcedure();
                break;
            case "tokenizer":
                section = new Sections.SectionTokenizer();
                break;
            default:
                Errors.Error.ReportError("Unknown section header: " + line);
                break;
        }

        if (section != null)
        {
            section.Compile();
            section.PostCompile();
        }
    }

    private static void ParseProcedure(Parser.Tokeniser stack)
    {
        // 2 tokens
        if (!stack.VerifyCount(2))
            return;

        // procedure
        stack.VerifyAndPop("procedure");

        // procedure name
        string name = stack.Pop();

        var procedure = new Process.ProcessProcedure(name);
        procedure.Compile();
    }

    private static void RunMain()
    {
        if (_process == null)
        {
            Errors.Error.ReportError("No process section");
            return;
        }

        var watch = Stopwatch.StartNew();

        try
        {
            // pre process
            foreach (var section in Symbols.SymbolTable.GetSections())
                section.PreProcess();

            // run
            _process.Run();

            // post process
            foreach (var section in Symbols.SymbolTable.GetSections())
                section.PostProcess();
        }
        catch (Exception e)
        {
            Errors.Error.ReportError("Runtime error: " + e.Message);
        }

        watch.Stop();

        TimeSpan ts = watch.Elapsed;
        string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        Console.WriteLine("RunTime " + elapsedTime);

        ReportStatistics();
    }

    private static void ReportStatistics()
    {
        var sections = Symbols.SymbolTable.GetSections();

        foreach (var section in sections)
            section.ReportStatistics();
    }
}