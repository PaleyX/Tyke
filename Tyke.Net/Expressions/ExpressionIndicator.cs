using System;

namespace Tyke.Net.Expressions
{
    internal class ExpressionIndicator : ExpressionBase
    {
        private enum IndicatorTypes
        {
            Undefined,
            Eof,
            Empty
        }

        private IndicatorTypes _indicatorType = IndicatorTypes.Undefined;

        internal ExpressionIndicator(Parser.Tokeniser stack)
        {
            ParseExpression(stack);
        }

        internal override bool Evaluate()
        {
            switch (_indicatorType)
            {
                case IndicatorTypes.Eof:
                    return Indicators.IsEof;
                case IndicatorTypes.Empty:
                    return Indicators.IsEmpty;
                default:
                    throw new ApplicationException("Unknown indicator found");
            }
        }

        internal sealed override void ParseExpression(Parser.Tokeniser stack)
        {
            if (!stack.VerifyCount(1))
                return;

            switch (stack.Pop())
            {
                case "eof":
                    _indicatorType = IndicatorTypes.Eof;
                    break;
                case "empty":
                    _indicatorType = IndicatorTypes.Empty;
                    break;
                default:
                    Errors.Error.SyntaxError("Unknown indicator expression");
                    break;
            }
        }
    }
}
