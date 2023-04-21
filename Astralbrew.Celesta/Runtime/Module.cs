using Astralbrew.Celesta.Data.SymbolDefinitions;
using System.Collections.Generic;

namespace Astralbrew.Celesta.Runtime
{
    public class Module : ImplementationsCollection
    {
        public new List<(FunctionDefinition Definition, IRuntimeImplementation Implementation)> Functions = new List<(FunctionDefinition Definition, IRuntimeImplementation Implementation)>();
        public new List<(OperatorDefinition Definition, IRuntimeImplementation Implementation)> Operators = new List<(OperatorDefinition Definition, IRuntimeImplementation Implementation)>();

        public override void RegisterOperator(string name, DataTypeDefinition argType1, DataTypeDefinition argType2, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var op = new OperatorDefinition(name, outputType, argType1, argType2);
            Operators.Add((op, implementation));
        }

        public override void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var fn = new FunctionDefinition(name, outputType, argTypes);
            Functions.Add((fn, implementation));
        }

        public override void RegisterOperator(OperatorDefinition definition, IRuntimeImplementation implementation)
            => Operators.Add((definition, implementation));

        public override void RegisterOperator(string name, DataTypeDefinition argType, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var op = new OperatorDefinition(name, outputType, argType);
            Operators.Add((op, implementation));
        }

        public override void RegisterFunction(FunctionDefinition definition, IRuntimeImplementation implementation)
            => Functions.Add((definition, implementation));
    }
}
