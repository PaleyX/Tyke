using System;
using System.Collections.Generic;
using System.Linq;
using Tyke.Net.Assign;

namespace Tyke.Net.Process;

internal class CommandBinaryAssign() : CommandBase(CommandTypes.BinaryAssign)
{
    private Data.DatafieldBinary _object;
    private readonly List<BinaryAssignBase> _postfix = [];

    internal override CommandBase Process()
    {
        var stack = new Stack<double>();

        foreach (var item in _postfix)
        {
            if (item is BinaryAssignOperator assignOperator)
            {
                if (stack.Count < 2)
                    throw new ApplicationException("CommandBinaryAssign::Process (A)");

                double dw1 = stack.Pop();
                double dw2 = stack.Pop();

                switch (assignOperator.Operator)
                {
                    case OperatorTypesB.Multiply:
                        stack.Push(dw2 * dw1);
                        break;
                    case OperatorTypesB.Divide:
                        if (dw1 == 0)
                            throw new DivideByZeroException();
                        stack.Push(dw2 / dw1);
                        break;
                    case OperatorTypesB.Mod:
                        if (dw1 == 0)
                            throw new DivideByZeroException();
                        stack.Push((uint)dw2 % (uint)dw1);
                        break;
                    case OperatorTypesB.Plus:
                        stack.Push(dw1 + dw2);
                        break;
                    case OperatorTypesB.Minus:
                        stack.Push(dw2 - dw1);
                        break;
                    default:
                        throw new ApplicationException("CommandBinaryAssign::Process (B)");
                }

                continue;
            }

            if (item is BinaryAssignDatafield datafield)
            {
                stack.Push(datafield.GetDWord());
                continue;
            }

            if (item is BinaryAssignConstant constant)
            {
                stack.Push(constant.GetDWord());
                continue;
            }

            throw new ApplicationException("CommandBinaryAssign::Process (C)");
        }

        if(stack.Count != 1)
            throw new ApplicationException("CommandBinaryAssign::Process (D)");

        // set
        _object.Set((uint)stack.Pop());

        return Next;
    }

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        // should have been checked before getting here
        if (stack.Count() < 3)
            throw new ApplicationException("CommandBinaryAssign::ParseCommand");

        // binary datafield
        _object = Symbols.SymbolTable.GetSymbol<Data.DatafieldBinary>(stack.Pop());

        // "="
        stack.VerifyAndPop("=");

        // postfix list
        var infix = new List<BinaryAssignBase>();

        // build postfix tokens list
        while (stack.Count() > 0)
        {
            string element = stack.Pop();

            // Operator
            if (IsOperator(element))
            {
                infix.Add(new BinaryAssignOperator(element));
                continue;
            }

            // numeric constant
            if (Tools.StringTools.CanConvert<uint>(element))
            {
                infix.Add(new BinaryAssignConstant(element));
                continue;
            }

            // must be binary datafield
            var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldBinary>(element);
            if (datafield != null)
            {
                infix.Add(new BinaryAssignDatafield(datafield));
            }
        }

        // infix to postfix
        var operators = new Stack<BinaryAssignOperator>();

        while (infix.Count > 0)
        {
            var item = infix.First();
            infix.RemoveAt(0);

            // datafield
            if (item is BinaryAssignDatafield)
            {
                _postfix.Add(item);
                continue;
            }

            // constant
            if (item is BinaryAssignConstant)
            {
                _postfix.Add(item);
                continue;
            }

            // must be operator
            var op = item as BinaryAssignOperator;
            if (op == null)
            {
                Errors.Error.SyntaxError("Bad operator order");
                continue;
            }

            if (op.Operator == OperatorTypesB.CloseBracket)
            {
                while (operators.Count > 0 && operators.Peek().Operator != OperatorTypesB.OpenBracket)
                {
                    _postfix.Add(operators.Pop());
                }

                if (operators.Count == 0 || operators.Peek().Operator != OperatorTypesB.OpenBracket)
                    Errors.Error.SyntaxError("Mismatched ')'");
                else
                    operators.Pop();
            }
            else
            if (op.Operator == OperatorTypesB.OpenBracket)
            {
                operators.Push(op);
            }
            else
            {
                while (operators.Count > 0 && operators.Peek().Order() >= op.Order())
                {
                    _postfix.Add(operators.Pop());
                }

                operators.Push(op);
            }
        }

        while (operators.Count > 0)
        {
            if (operators.Peek().Operator == OperatorTypesB.OpenBracket)
                Errors.Error.SyntaxError("Mismatched '('");
            else
                _postfix.Add(operators.Peek());

            operators.Pop();
        }

        ValidateOrder();
    }

    private void ValidateOrder()
    {
        long dw = 0;

        foreach (var item in _postfix)
        {
            if (item is BinaryAssignOperator)
            {
                if (dw < 2)
                    Errors.Error.SyntaxError("Bad operator.operand sequence");
                dw -= 1; // eqv pop:pop:push
            }
            else
                ++dw;
        }

        if (dw != 1)
            Errors.Error.SyntaxError("Bad operator/operand sequence");
    }

    private bool IsOperator(string element)
    {
        if (element.Length != 1)
            return false;

        char c = element[0];

        if (Parser.Tokeniser.IsMathOperator(c))
            return true;

        if (c == '(' || c == ')')
            return true;

        return false;
    }
}