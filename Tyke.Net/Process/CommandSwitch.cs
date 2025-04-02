using System.Collections.Generic;

namespace Tyke.Net.Process
{
    internal class CommandSwitch() : CommandBase(CommandTypes.Switch)
    {
        private CommandCase _default;
        private CommandBase _endSwitch;

        private readonly List<CommandCase> _cases = [];

        internal override CommandBase Process()
        {
            if (_cases.Count == 0)
                Errors.Error.ReportError("Empty case list on switch");

            // evaluate case expressions
            foreach (CommandCase command in _cases)
            {
                if (command.Expression.Evaluate())
                    return command.Next;
            }

            // no matching case - if default go to its nop else
            // jump to endswitch
            if (_default != null)
            {
                return _default.Next;
            }

            return _endSwitch;
        }

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            if (!stack.VerifyCount(1))
                return;

            stack.VerifyAndPop("switch");

            Symbols.ProcessStack.Push(this);
        }

        internal void CaseToEndSwitch(CommandEndSwitch es)
        {
            if (_cases.Count == 0)
                Errors.Error.SyntaxError("No case commands");

            // case list
            _cases.ForEach(c => c.Jump = es);

            // default
            if (_default != null)
                _default.Jump = es;

            // end switch
            _endSwitch = es;
        }

        internal void AddCase(CommandCase command)
        {
            _cases.Add(command);
        }

        internal void AddDefault(CommandCase command)
        {
            _default = command;
        }
    }
}
