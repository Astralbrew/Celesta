using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime
{
    public class RuntimeContext
    {
        Dictionary<VariableDefinition, object> Variables = new Dictionary<VariableDefinition, object>();

        Dictionary<FunctionDefinition, IRuntimeImplementation> Functions = new Dictionary<FunctionDefinition, IRuntimeImplementation>();

        Dictionary<OperatorDefinition, IRuntimeImplementation> Operators = new Dictionary<OperatorDefinition, IRuntimeImplementation>();

    }
}
