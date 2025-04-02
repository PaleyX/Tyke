namespace Tyke.Net.Process
{
    internal class CommandCase() : CommandBranchBase(CommandTypes.Case)
    {
        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            bool isDefault = false;

            // first must be 'case' - unless it's default
            // in which case it's the only word
            switch (stack.Pop())
            {
                case "case":
                    isDefault = false;
                    break;
                case "default":
                    isDefault = true;
                    stack.VerifyEmpty();
                    break;
                default:
                    Errors.Error.SyntaxError();
                    break;
            }

            // expression
            if (!isDefault)
                base.Expression = Expressions.ExpressionBase.GetExpression(stack);

            // top of stack must be a switch
            var x = Symbols.ProcessStack.Peek();
            if (x == null)
            {
                Errors.Error.SyntaxError("No switch command found");
                return;
            }

            if (x.CommandType != CommandTypes.Switch)
            {
                Errors.Error.SyntaxError("No switch command found");
                return;
            }

            // if 'case' add to switch case list
            // if 'default' make default
            if (isDefault)
            {
                (x as CommandSwitch).AddDefault(this);
            }
            else
            {
                (x as CommandSwitch).AddCase(this);
            }
        }

        internal Expressions.ExpressionBase Expression => base.Expression;

        internal override CommandBase Process()
        {
            return Jump;
        }
    }
}
