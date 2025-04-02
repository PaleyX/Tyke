using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Tyke.Net.Assign;

namespace Tyke.Net.Process
{
    internal class CommandAlphaAssign() : CommandBase(CommandTypes.AlphaAssign)
    {
        private Data.DatafieldAlpha _object;
        private readonly List<AlphaAssignBase> _postfix = [];

        internal override CommandBase Process()
        {
            if (_postfix.Count == 1)
                ProcessSimple();
            else
                ProcessExtended();

            return Next;
        }

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // should have been checked before getting here
            if (stack.Count() < 3)
                throw new ApplicationException("CommandAlphaAssign::ParseCommand");

            _object = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(stack.Pop());
            if (_object == null)
                return;

            // =
            stack.VerifyAndPop("=");

            // build tokens list
            while (stack.Count() > 0)
            {
                string element = stack.Pop();

                // operator
                if (IsOperator(element))
                {
                    _postfix.Add(new AlphaAssignOperator(element));
                    continue;
                }

                // alpha constant
                if (Tools.StringTools.IsQuotedString(element))
                {
                    _postfix.Add(new AlphaAssignConstant(element));
                    continue;
                }

                // must be a alpha datafield
                var datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(element);
                if (datafield != null)
                {
                    _postfix.Add(new AlphaAssignDatafield(datafield));
                }
            }
            
            // Validate
            ValidateOrder();
        }

        private bool IsOperator(string element)
        {
            return element == "+";
        }

        private void ValidateOrder()
        {
            if (!_postfix.Any())
            {
                Errors.Error.SyntaxError();
                return;
            }

            Debug.Assert(_postfix.Any());

            int l = 1;

            foreach (var item in _postfix)
            {
                if ((l++ % 2) == 0)
                {
                    if (!(item is AlphaAssignOperator))
                    {
                        Errors.Error.SyntaxError("Expected operator");
                    }
                }
                else
                {
                    if (item is AlphaAssignConstant || item is AlphaAssignDatafield)
                    {
                        // fine
                    }
                    else
                        Errors.Error.SyntaxError(Errors.StdErrors.ExpectedAlpha);
                }
            }

            if (_postfix.Last() is AlphaAssignOperator)
                Errors.Error.SyntaxError();
        }

        private void ProcessSimple()
        {
            Debug.Assert(_postfix.Count == 1);

            var element = _postfix.FirstOrDefault();
            Debug.Assert(element != null);

            if (element is AlphaAssignDatafield datafield)
            {
                _object.Set(datafield.Datafield);
                return;
            }

            if (element is AlphaAssignConstant constant)
            {
                _object.Set(constant.Constant);
                return;
            }

            throw new ApplicationException("CommandAlphaAssign::ProcessSimple");
        }

        private void ProcessExtended()
        {
            StringBuilder sb = new StringBuilder();

            // All this assumes the only operator is "="
            foreach (var item in _postfix)
            {
                if (item is AlphaAssignDatafield datafield)
                {
                    sb.Append(datafield.Datafield.ToString());
                    continue;
                }

                if(item is AlphaAssignConstant constant)
                {
                    sb.Append(constant.Constant);
                    continue;
                }

                if(item is AlphaAssignOperator)
                {
                    continue;
                }

                throw new ApplicationException("CommandAlphaAssign::ProcessExtended");
            }

            _object.Set(sb.ToString());
        }
    }
}
