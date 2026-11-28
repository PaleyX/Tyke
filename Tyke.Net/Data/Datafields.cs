using System;

namespace Tyke.Net.Data;

internal static class Datafields
{
    private enum DatafieldElement
    {
        Name = 1,
        Equals = 2,
        Offset = 3,
        Length = 4,
        Type = 5,
        Unused = 6,
        Constant = 7
    }

    internal static void Compile()
    {
        var start = 1;

        while (true)
        {
            var line = Parser.TykeFile.GetLine();

            if (string.Equals(line, "end", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            var datafield = CompileDatafield(line, start);

            if (datafield != null)
                start = datafield.Offset + 1 +  datafield.Length;
        }
    }

    private static DatafieldBase CompileDatafield(string line, int start)
    {
        var tokeniser = new Parser.Tokeniser(line);

        string name = null;
        string offset = null;
        string length = null;
        string type = "a";
        string constant = null;

        int index = 1;
        bool gotType = false;
        bool gotConst = false;
        
        while(tokeniser.Count() > 0)
        {
            var token = tokeniser.Pop();

            if (token == "/")
            {
                ++index;
                continue;
            }

            var element = (DatafieldElement)index;

            switch(element)
            {
                case DatafieldElement.Name:
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                            Errors.Error.SyntaxError();
                        name = token;
                        ++index;
                    }
                    break;
                case DatafieldElement.Equals:
                    {
                        if (token != "=")
                            Errors.Error.SyntaxError();
                        ++index;
                    }
                    break;
                case DatafieldElement.Offset:
                    {
                        if (!string.IsNullOrWhiteSpace(offset))
                            Errors.Error.SyntaxError();
                        offset = token == "*" ? start.ToString() : token;
                    }
                    break;
                case DatafieldElement.Length:
                    {
                        if (!string.IsNullOrWhiteSpace(length))
                            Errors.Error.SyntaxError();
                        length = token;
                    }
                    break;
                case DatafieldElement.Type:
                    {
                        if (gotType)
                            Errors.Error.SyntaxError();
                        type = token;
                        gotType = true;
                    }
                    break;
                case DatafieldElement.Unused:
                    Errors.Error.SyntaxError("Unused element");
                    break;
                case DatafieldElement.Constant:
                    {
                        if (!string.IsNullOrWhiteSpace(constant))
                            Errors.Error.SyntaxError();
                        constant = token;
                        gotConst = true;
                    }
                    break;
                default:
                    Errors.Error.SyntaxError("Unknown datafield element: " + token);
                    break;
            }
        }

        DatafieldBase datafield = type switch
        {
            "a" => new DatafieldAlpha(DataBuffer.CurrentBuffer, name, offset, length),
            "b" => new DatafieldBinary(DataBuffer.CurrentBuffer, name, offset, length),
            _ => null
        };

        if (datafield is not null)
        {
            if (datafield.CheckAndValidateElements())
            {
                if (gotConst)
                    datafield.SetConstant(constant);
            }
        }
        else
        {
            Errors.Error.SyntaxError($"Unsupported field type [{type}]");
        }

        return datafield;
    }
}