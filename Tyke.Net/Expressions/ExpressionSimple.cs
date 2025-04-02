using System.Diagnostics;

namespace Tyke.Net.Expressions
{
    internal class ExpressionSimple<T1, T2> : ExpressionBase where T1: Data.DatafieldBase where T2: Data.OperandBase, new()
    {
        private enum CompareTypes
        {
            Undefined,
            Equal,
            NotEqual,
            Greater,
            GreaterEqual,
            Less,
            LessEqual
        }

        T1 _datafield;
        T2 _operand;

        internal ExpressionSimple(Parser.Tokeniser stack)
        {
            CompareType = CompareTypes.Undefined;
            ParseExpression(stack);
        }

        internal override bool Evaluate()
        {
            Debug.Assert(_datafield != null);
            Debug.Assert(_operand.Valid());

            switch (CompareType)
            {
                case CompareTypes.Equal:
                    return _operand.CompareEq(_datafield);
                case CompareTypes.NotEqual:
                    return _operand.CompareNe(_datafield);
                case CompareTypes.Greater:
                    return _operand.CompareGt(_datafield);
                case CompareTypes.GreaterEqual:
                    return _operand.CompareGe(_datafield);
                case CompareTypes.Less:
                    return _operand.CompareLt(_datafield);
                case CompareTypes.LessEqual:
                    return _operand.CompareLe(_datafield);
            }

            Errors.Error.ReportError("Unknown comparison type");
            return false;
    //switch(opComp)
    //{
    //case opcEqual:
    //    return m_op == m_cdTest;
    //case opcNotEqual:
    //    return !(m_op == m_cdTest);
    //case opcGreater:
    //    return m_op < m_cdTest;	
    //case opcGreaterEqual:
    //    return m_op == m_cdTest || m_op < m_cdTest;
    //case opcLess:
    //    return m_op > m_cdTest;
    //case opcLessEqual:
    //    return m_op == m_cdTest || m_op > m_cdTest;
    //default:
    //    CError::ReportError("Unknown comparison type");
    //    return false;
    //}
        }

        internal sealed override void ParseExpression(Parser.Tokeniser stack)
        {
            // 3 tokens
            if (!stack.VerifyCount(3))
                return;

            // next is datafield
            var token = stack.Pop();
            _datafield = Symbols.SymbolTable.GetSymbol<T1>(token);

            // next is comparision operator
            token = stack.Pop();

            switch (token)
            {
                case "=":
                    CompareType = CompareTypes.Equal;
                    break;
                case "<>":
                    CompareType = CompareTypes.NotEqual;
                    break;
                case ">=":
                    CompareType = CompareTypes.GreaterEqual;
                    break;
                case ">":
                    CompareType = CompareTypes.Greater;
                    break;
                case "<":
                    CompareType = CompareTypes.Less;
                    break;
                case "<=":
                    CompareType = CompareTypes.LessEqual;
                    break;
                default:
                    Errors.Error.ReportError("Unknown comparison operator");
                    return;
            }

            // next is literal or datafield
            token = stack.Pop();

            _operand = new T2();
            _operand.SetFromToken(token);
        }

        private CompareTypes CompareType { get; set; }

    }

}
