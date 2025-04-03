using System;

namespace Tyke.Net.Errors;

internal enum StdErrors
{
    ExpectedDatafield,
    ExpectedAlpha,
    UnknownOption,
    UnknownDatafieldType,
    IncompatibleOperands,
    ExpectedAlphaDatafield,
    ExpectedAlphaLiteral
}

internal static class Error
{
    private static int _errorCount;

    internal static void ReportError(string line)
    {
        Console.WriteLine("Error: {0}", line);
        ++_errorCount;
    }

    internal static void ReportError(string line, params object[] args)
    {
        ReportError(string.Format(line, args));
    }

    internal static void SyntaxError(string line = null)
    {
        if (string.IsNullOrEmpty(line))
            ReportError("Syntax error");
        else
            ReportError("Syntax error: " + line);
    }

    internal static void SyntaxError(string line, params object[] args)
    {
        ReportError("Syntax error: " + line, args);
    }

    internal static void SyntaxError(StdErrors stderror)
    {
        switch (stderror)
        {
            case StdErrors.ExpectedDatafield:
                SyntaxError("Expected datafield");
                break;
            case StdErrors.ExpectedAlpha:
                SyntaxError("Expected alpha datafield or constant");
                break;
            case StdErrors.UnknownOption:
                SyntaxError("Unknown option");
                break;
            case StdErrors.UnknownDatafieldType:
                SyntaxError("Unknown datafield type");
                break;
            case StdErrors.IncompatibleOperands:
                SyntaxError("incompatibe operands");
                break;
            case StdErrors.ExpectedAlphaDatafield:
                SyntaxError("Expected alpha datafield");
                break;
            case StdErrors.ExpectedAlphaLiteral:
                SyntaxError("Expected alpha literal");
                break;
            default:
                throw new NotImplementedException("Unknown stderror");
        }
    }

    internal static void NonTerminatingRuntimeError(string message, params object[] args)
    {
        Console.WriteLine(message, args);
    }

    internal static bool HasErrors => _errorCount > 0;

    internal static int ErrorCount => _errorCount;
}