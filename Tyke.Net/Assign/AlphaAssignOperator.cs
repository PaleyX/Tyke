using System;

namespace Tyke.Net.Assign
{
    internal enum OperatorTypesA
    {
        Concatenate
    }

    internal class AlphaAssignOperator : AlphaAssignBase
    {
        internal AlphaAssignOperator(string element)
        {
            if (element == "+")
                Operator = OperatorTypesA.Concatenate;
            else
            {
                throw new ApplicationException("Unknown operator in Alpha Assign");
            }
        }

        internal OperatorTypesA Operator
        {
            get;
            private set;
        }
    }
}
