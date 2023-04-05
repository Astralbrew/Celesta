using System;

namespace Astralbrew.Celesta.Runtime.Implementation.Operators
{
    internal class BinaryOperator : IRuntimeImplementation
    {
        private Func<object[], object> Function;

        public BinaryOperator(Func<object[], object> function)
        {
            Function = function;
        }

        public object Execute(object[] input) => Function(input);
    }
}
