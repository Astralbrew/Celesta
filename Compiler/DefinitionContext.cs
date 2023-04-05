using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Compiler
{
    public class DefinitionContext
    {
        HashSet<VariableDefinition> Variables = new HashSet<VariableDefinition>();
        HashSet<DataTypeDefinition> DataTypes = new HashSet<DataTypeDefinition>();
        HashSet<FunctionDefinition> Functions = new HashSet<FunctionDefinition>();
        HashSet<OperatorDefinition> Operators = new HashSet<OperatorDefinition>();

        public DefinitionContext()
        {
            LanguageDefinition.PrimitiveTypes.Enumerate().ToList().ForEach(RegisterDataType);            
        }
        
        public void RegisterDataType(DataTypeDefinition dataType)
        {
            if(DataTypes.Contains(dataType))
            {
                throw new DuplicateDefinitionException($"Duplicate definition of data type: '{dataType}'");
            }
            DataTypes.Add(dataType);
        }

        public void RegisterVariable(VariableDefinition var)
        {
            if(Variables.Contains(var))
            {
                throw new DuplicateDefinitionException($"Duplicate definition of variable: '{var}'");
            }
            ValidateType(var.Type);
            
            Variables.Add(var);
        }

        public void RegisterVariable(string name, string type)
        {
            RegisterVariable(new VariableDefinition(name, GetDataType(type, true)));
        }

        public FunctionDefinition RegisterFunction(FunctionDefinition func)
        {
            if(Functions.Contains(func))
            {
                throw new DuplicateDefinitionException($"Duplicate definition of function: '{func}'");
            }
            
            func.GetArgumentTypes(includeOutput:true).ForEach(ValidateType);

            Functions.Add(func);

            return func;
        }

        public FunctionDefinition RegisterFunction(string name, params string[] types)
        {
            var dtypes = types.Select(t => GetDataType(t, true)).ToArray();
            return RegisterFunction(new FunctionDefinition(name, dtypes.Last(), dtypes.Take(dtypes.Count() - 1).ToArray()));
        }

        public OperatorDefinition RegisterOperator(OperatorDefinition op)
        {
            if (Operators.Contains(op))
            {
                throw new DuplicateDefinitionException($"Duplicate definition of operator: '{op}'");
            }

            op.GetArgumentTypes(includeOutput: true).ForEach(ValidateType);

            Operators.Add(op);
            return op;
        }

        public OperatorDefinition RegisterOperator(string symbol, string type1, string type2, string typeO)
        {
            return RegisterOperator(new OperatorDefinition(symbol,
                GetDataType(typeO, true),
                GetDataType(type1, true),
                GetDataType(type2, true)));
        }

        public OperatorDefinition RegisterOperator(string symbol, string typeI, string typeO)
        {
            return RegisterOperator(new OperatorDefinition(symbol,
                GetDataType(typeO, true),
                GetDataType(typeI, true)));
        }

        private void ValidateType(DataTypeDefinition t)
        {
            if (!DataTypes.Contains(t))
                throw new IdenitifierNotFoundException($"Cannot find definition for type '{t?.Name ?? "<Null>"}'");
        }

        private D GetDefinition<D>(IEnumerable<D> defs, Func<D, bool> pred, bool throwOnNotFound = false)
        {
            var result = defs.Where(pred).FirstOrDefault();
            if (throwOnNotFound && result == null)
                throw new IdenitifierNotFoundException($"Error getting {typeof(D).Name} instance");
            return result;
        }

        private D GetDefinition<D>(IEnumerable<D> defs, Func<D, bool> pred, bool throwOnNotFound = false, string message = "")
        {
            var result = defs.Where(pred).FirstOrDefault();
            if (throwOnNotFound && result == null)
                throw new IdenitifierNotFoundException($"Error getting {typeof(D).Name} instance : {message}");
            return result;
        }

        public DataTypeDefinition GetDataType(string name, bool throwOnNotFound = false)
            => GetDefinition(DataTypes, d => d.Name == name, throwOnNotFound, name);
        public VariableDefinition GetVariable(string name, bool throwOnNotFound = false)
            => GetDefinition(Variables, v => v.Name == name, throwOnNotFound, name);


        public OperatorDefinition GetOperator(string name, DataTypeDefinition argType1, DataTypeDefinition argType2,
            bool throwOnNotFound = false)
        {
            var x = Operators;
            var op = Operators.Where(o => o.Name == name && o.ArgType1 == argType1 && o.ArgType2 == argType2)
                .FirstOrDefault();            
            if (op == null && throwOnNotFound) 
                throw new IdenitifierNotFoundException($"Operator {name}({argType1},{argType2}) not found");
            return op;
        }

        public OperatorDefinition GetOperator(string name, DataTypeDefinition argType1, bool throwOnNotFound = false)
        {
            var op = Operators.Where(o => o.IsUnary && o.Name == name && o.ArgType1 == argType1)
                .FirstOrDefault();
            if (op == null && throwOnNotFound)
                throw new IdenitifierNotFoundException($"Operator {name}({argType1}) not found");
            return op;

        }

        public FunctionDefinition GetFunction(string name, DataTypeDefinition[] argTypes, bool throwOnNotFound)
        {            
            var fn = Functions.Where(f => f.Name == name && f.GetArgumentTypes().SequenceEqual(argTypes)).FirstOrDefault();
            if (fn == null && throwOnNotFound)
                throw new IdenitifierNotFoundException($"Function {name}({string.Join(", ", (IEnumerable<DataTypeDefinition>)argTypes)}) not found");
            return fn;
        }

        public bool IsFunction(string name) => Functions.Any(f => f.Name == name);
    }
}
