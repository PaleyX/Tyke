using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tyke.Net.Process;

internal abstract class ProcessBase : Symbols.SymbolBase
{
    private CommandBase _first;
    private CommandBase _last;

    private readonly List<CommandBase> _commands = [];

    internal void Compile()
    {
        while (true)
        {
            string line = Parser.TykeFile.GetLine();

            if (line.ToLower() == "end")
                break;

            var stack = new Parser.Tokeniser(line);

            CommandBase command = null;
            string name = stack.Peek();
                
            switch (name)
            {
                case "loop":
                    command = AddCommand<CommandLoop>();
                    break;
                case "endloop":
                    command = AddCommandWithNop<CommandEndLoop>();
                    break;
                case "inc":
                    command = AddCommand<CommandInc>();
                    break;
                case "display":
                    command = AddCommand<CommandDisplay>();
                    break;
                case "until":
                    command = AddCommand<CommandUntil>();
                    break;
                case "if":
                    command = AddCommand<CommandIf>();
                    break;
                case "endif":
                    command = AddCommand<CommandEndif>();
                    break;
                case "else":
                    command = AddCommandWithNop<CommandElse>();
                    break;
                case "ifnot":
                    command = AddCommand<CommandIfNot>();
                    break;
                case "move": 
                    command = AddCommand<CommandMove>();
                    break;
                case "open":
                    command = AddCommand<CommandOpen>();
                    break;
                case "close":
                    command = AddCommand<CommandClose>();
                    break;
                case "read":
                    command = AddCommand<CommandRead>();
                    break;
                case "write":
                    command = AddCommand<CommandWrite>();
                    break;
                case "cast":
                    command = AddCommand<CommandCast>();
                    break;
                case "switch":
                    command = AddCommand<CommandSwitch>();
                    break;
                case "endswitch":
                    command = AddCommand<CommandEndSwitch>();
                    break;
                case "case":
                    command = AddCommandWithNop<CommandCase>();
                    break;
                case "default":
                    command = AddCommand<CommandCase>();
                    break;
                case "modify":
                    command = AddCommand<CommandModify>();
                    break;
                case "put":
                    command = AddCommand<CommandPut>();
                    break;
                case "call":
                    command = AddCommand<CommandCall>();
                    break;
                case "get":
                    command = AddCommand<CommandGet>();
                    break;
                case "do":
                    command = AddCommand<CommandDo>();
                    break;
                default:
                    if (Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(name) != null)
                    {
                        command = ParseLeadingDatafieldStatement(stack);
                    }
                    else
                    {
                        Errors.Error.SyntaxError("Unknown process section command: {0}", line);
                    }
                    break;
            }

            if (command != null)
            {
                _commands.Add(command);
                command.ParseCommand(stack);
            }
        }
    }

    internal IEnumerable<CommandBase> GetCommands()
    {
        return _commands;
    }

    private CommandBase ParseLeadingDatafieldStatement(Parser.Tokeniser stack)
    {
        // must be at least 3 tokens
        if (stack.Count() < 3)
        {
            Errors.Error.SyntaxError();
            return null;
        }

        // okey dokey, pig in a pokey
        var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Peek(), false);
        Debug.Assert(datafield != null);

        switch (datafield.DatafieldType)
        {
            case Data.DatafieldBase.DatafieldTypes.Alpha:
                return AddCommand<CommandAlphaAssign>(); 
            case Data.DatafieldBase.DatafieldTypes.Binary:
                return AddCommand<CommandBinaryAssign>();
            default:
                throw new ApplicationException("ParseLeadingDatafieldStatement");
        }
    }

    internal void Run()
    {
        if (_first == null)
            return;

        CommandBase next = _first.Process();

        while (next != null)
        {
            next = next.Process();
        }
    }

    private T AddCommand<T>() where T: CommandBase, new()
    {
        T command = new T();

        _first ??= command;

        if (_last != null)
            _last.Next = command;
        _last = command;

        return command;
    }

    private T AddCommandWithNop<T>() where T : CommandBase, new()
    {
        T command = AddCommand<T>();
        AddCommand<CommandNop>();

        return command;
    }
}