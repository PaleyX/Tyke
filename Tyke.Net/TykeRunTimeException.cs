using System;

namespace Tyke.Net;

public class TykeRunTimeException : Exception
{
    public TykeRunTimeException(string message, string name) 
        : base($"{message} - {name}")
    {

    }

    public TykeRunTimeException(string message)
        : base(message)
    {
    }
}