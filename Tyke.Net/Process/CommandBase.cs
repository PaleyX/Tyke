namespace Tyke.Net.Process;

internal abstract class CommandBase
{
    internal enum CommandTypes
    {
        Loop,
        EndLoop,
        Until,
        Nop,
        Inc,
        Display,
        If,
        Endif,
        Else,
        IfNot,
        Move,
        BinaryAssign,
        AlphaAssign,
        Open,
        Close,
        Read,
        Write,
        Cast,
        Switch,
        Case,
        EndSwitch,
        Modify,
        Put,
        Call,
        Get,
        Do
    }

    internal CommandBase(CommandTypes commandType)
    {
        CommandType = commandType;
    }

    internal CommandTypes  CommandType { get; }

    internal abstract void ParseCommand(Parser.Tokeniser stack);
        
    internal virtual CommandBase Process() 
    {
        return Next;
    }

    internal CommandBase Next {get; set;}
}