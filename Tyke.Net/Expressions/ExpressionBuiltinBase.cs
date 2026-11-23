using System;

namespace Tyke.Net.Expressions;

internal abstract class ExpressionBuiltinBase : ExpressionBase
{
    protected Func<bool> Func;

    protected enum TestTypes
    {
        Is,
        Not
    }

    protected TestTypes TestType;

    internal override bool Evaluate()
    {
        bool result = Func();

        if (TestType == TestTypes.Not)
            result = !result;

        return result;
    }

    protected void GetTestType(string token)
    {
        TestType = token switch
        {
            "is" => TestTypes.Is,
            "not" => TestTypes.Not,
            _ => throw new TykeRunTimeException("ExpressionBuiltinBase:ParseExpression")
        };
    }
}