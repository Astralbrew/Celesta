using System;

namespace Astralbrew.Celesta.Runtime.Implementation
{
    internal class FunctionImplementation : IRuntimeImplementation
    {
        private Func<object[], object> Function;

        public FunctionImplementation(Func<object[], object> function)
        {
            Function = function;
        }

        public FunctionImplementation(Func<object> function)
        {
            Function = o => function();
        }

        public FunctionImplementation(Func<object, object> function)
        {
            Function = o => function(o[0]);
        }

        public FunctionImplementation(Func<object, object, object> function)
        {
            Function = o => function(o[0], o[1]);
        }

        public FunctionImplementation(Func<object, object, object, object> function)
        {
            Function = o => function(o[0], o[1], o[2]);
        }

        public FunctionImplementation(Func<object, object, object,object, object> function)
        {
            Function = o => function(o[0], o[1], o[2], o[3]);
        }

        public object Execute(object[] input) => Function(input);
    }
}
