using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime.Implementation
{
    internal class BinaryOperator : IRuntimeImplementation
    {
        Func<object, object, object> Operation;
        public BinaryOperator(Func<object, object, object> operation)
        {
            Operation = operation;
        }

        public object Execute(object[] input) => Operation(input[0], input[1]);        
    }
}
