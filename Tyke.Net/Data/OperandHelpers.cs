using System;

namespace Tyke.Net.Data;

internal static class OperandHelpers
{
    internal static OperandBase AllocateOpFromToken(string token, bool mustBeDatafield = false)
    {
        var datafield = Symbols.SymbolTable.GetSymbol<DatafieldBase>(token, false);
        if (datafield == null)
        {
            if (mustBeDatafield)
            {
                Errors.Error.SyntaxError(Errors.StdErrors.ExpectedDatafield);
                return null;
            }

            // Alpha
            if (Tools.StringTools.IsQuotedString(token))
            {
                var op = new OperandAlpha();
                op.SetFromToken(token);
                return op;
            }

            // binary
            if (Tools.StringTools.CanConvert<uint>(token))
            {
                var op = new OperandBinary();
                op.SetFromToken(token);
                return op;
            }

            Errors.Error.SyntaxError();
            return null;
        }

        return datafield switch
        {
            DatafieldAlpha alpha => new OperandAlpha(alpha),
            DatafieldBinary binary => new OperandBinary(binary),
            _ => throw new ApplicationException("OperandHelpers::AllocateOpFromToken")
        };
    }

    internal static bool Compatible(OperandBase operand, DatafieldBase datafield)
    {
        if (operand == null || datafield == null)
            throw new ApplicationException("OperandHelpers::Compatible (A)");

        if (operand is OperandAlpha)
            return datafield is DatafieldAlpha;

        if (operand is OperandBinary)
            return datafield is DatafieldBinary;

        throw new ApplicationException("OperandHelpers::Compatible (A)");
    }
}