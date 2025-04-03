using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Tyke.Net.Parser;

internal class Tokeniser
{
    private Stack<string> _tokens;
    private readonly bool _reportNonEmpty;
    private readonly TykeLine _line;

    internal Tokeniser(string line, bool reportNonEmpty = true)
    {
        _line = new TykeLine(line);

        Tokenize();
    }

    internal string Pop()
    {
        if (Count() == 0)
            throw new ApplicationException("Tokenizer::Pop");

        string token = _tokens.Pop();

        return token;
    }

    internal string Peek (int tokens = 1)
    {
        Debug.Assert(tokens > 0);

        if (Count() < tokens)
            throw new ApplicationException("Tokenizer::Peek");

        if (tokens == 1)
            return _tokens.Peek();

        return _tokens.ElementAt(tokens - 1);
    }

    internal int Count()
    {
        return _tokens.Count;
    }

    internal bool VerifyCount (int required, bool reportError = true)
    {
        if(Count() != required && reportError)
        {
            Errors.Error.SyntaxError("Invalid token count, have {0} require {1}", Count(), required);
        }

        return Count() == required;
    }

    internal bool VerifyAndPop(string required)
    {
        string got = Pop();
        if(got == required)
            return true;

        Errors.Error.SyntaxError("Expected '{0}' - have '{1}'", required, got);
        return false;
    }

    internal bool VerifyEmpty(bool reportError = true)
    {
        if (reportError && Count() != 0)
        {
            Errors.Error.SyntaxError("Invalid token count");
        }

        return Count() == 0;
    }

    internal IEnumerable<string> GetList(string seperator = ",")
    {
        var count = 0;

        while (Count() > 0)
        {
            ++count;

            if (count % 2 == 0)
                VerifyAndPop(seperator);
            else
                yield return Pop();
        }
    }

    internal static bool IsMathOperator(char c)
    {
        return @"*+-\%".Contains(c);
    }

    internal static bool IsStdBreak(char c)
    {
        return "/,.:()".Contains(c);
    }

    internal static bool IsOperator(char c)
    {
        return "=><".Contains(c);
    }

    private void Tokenize()
    {
        Debug.Assert(_tokens == null);

        var list = new List<string>();
        string token;
        while ((token = GetToken()) != string.Empty)
            list.Add(PostProcessToken(token));

        list.Reverse();

        _tokens = new Stack<string>(list);
    }

    private string GetToken()
    {
        // eat whitespace
        while(_line.Eol() == false && char.IsWhiteSpace(_line.Peek()))
            _line.Pop();

        if (_line.Eol())
            return string.Empty;

        // quoted token?
        if (_line.Peek() == '"')
            return GetQuoted();

        // get token until whitespace or standard break
        var token = new StringBuilder();

        while (!_line.Eol())
        {
            if (IsStdBreak(_line.Peek()))
            {
                if (token.Length == 0)
                    token.Append(_line.Pop());
                break;
            }

            // operators
            if (IsOperator(_line.Peek()))
            {
                if(token.Length == 0)
                    token.Append(GetOperator());
                break;
            }

            // Maths?
            if(IsMathOperator(_line.Peek()))
            {
                if(token.Length == 0)
                {
                    token.Append(_line.Pop());
                    break;
                }
            }

            // whitespace
            if(char.IsWhiteSpace(_line.Peek()))
            {
                if(token.Length > 0)
                    break;
            }

            token.Append(_line.Pop());
        }
            
        return token.ToString();
    }

    private string GetOperator()
    {
        string token = null;

        while (!_line.Eol() && IsOperator(_line.Peek()))
            token += _line.Pop();

        return token;
    }

    private string GetQuoted()
    {
        Debug.Assert(_line.Peek() == '"');

        var token = new StringBuilder();

        token.Append(_line.Pop());

        while (!_line.Eol())
        {
            token.Append(_line.Peek());
            if (_line.Peek() == '"')
            {
                _line.Pop();
                break;
            }

            _line.Pop();
        }

        return token.ToString();
    }

    private string PostProcessToken(string token)
    {
        string result = token;

        if (result.Length == 0)
            return result;


        //// Parameter ?
        //if(*sReturn.c_str() == '@')
        //{
        //    long l = atol(sReturn.c_str() + 1);
        //    sReturn = CParams::Find(l);
        //}

        // builtin literals
        switch (result)
        {
            case "spaces":
            case "space":
                return "\"\"";
            case "zeros":
            case "zero":
                return "0";
            case "quote":
                return "\"";
            case "tab":
                return "\t";
            case "bel":
                return "\a";
            case "crlf":
                return Environment.NewLine;
        }

        return result;
    }
}