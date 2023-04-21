using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Runtime.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Runtime
{
    public abstract class ImplementationsCollection
    {
        protected Dictionary<FunctionDefinition, IRuntimeImplementation> Functions = new Dictionary<FunctionDefinition, IRuntimeImplementation>();

        protected Dictionary<OperatorDefinition, IRuntimeImplementation> Operators = new Dictionary<OperatorDefinition, IRuntimeImplementation>();

        public abstract void RegisterOperator(OperatorDefinition definition, IRuntimeImplementation implementation);
        public abstract void RegisterOperator(string name, DataTypeDefinition argType1, DataTypeDefinition argType2, DataTypeDefinition outputType, IRuntimeImplementation implementation);
        public abstract void RegisterOperator(string name, DataTypeDefinition argType, DataTypeDefinition outputType, IRuntimeImplementation implementation);
        public abstract void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, IRuntimeImplementation implementation);
        public abstract void RegisterFunction(FunctionDefinition definition, IRuntimeImplementation implementation);


        public void RegisterOperator(OperatorDefinition definition, Func<object, object, object> operation)
            => RegisterOperator(definition, new FunctionImplementation(operation));

        public void RegisterOperator(OperatorDefinition definition, Func<object, object> operation)
            => RegisterOperator(definition, new FunctionImplementation(operation));

        public void RegisterOperator(string name, DataTypeDefinition argType1, DataTypeDefinition argType2, DataTypeDefinition outputType, Func<object, object, object> operation)
            => RegisterOperator(name, argType1, argType2, outputType, new FunctionImplementation(operation));

        public void RegisterOperator(string name, DataTypeDefinition argType, DataTypeDefinition outputType, Func<object, object> operation)
            => RegisterOperator(name, argType, outputType, new FunctionImplementation(operation));


        public void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, Func<object> func)
            => RegisterFunction(name, argTypes, outputType, new FunctionImplementation(func));

        public void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, Func<object,object> func)
            => RegisterFunction(name, argTypes, outputType, new FunctionImplementation(func));

        public void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, Func<object, object, object> func)
            => RegisterFunction(name, argTypes, outputType, new FunctionImplementation(func));

        public void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, Func<object, object, object, object> func)
            => RegisterFunction(name, argTypes, outputType, new FunctionImplementation(func));
    }
}
