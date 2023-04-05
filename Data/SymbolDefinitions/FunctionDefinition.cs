using Astralbrew.Celesta.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class FunctionDefinition : AbstractSymbolDefinition
    {
        public DataTypeDefinition ResultType { get; }
        private DataTypeDefinition[] ArgTypes;        

        public FunctionDefinition(string name, DataTypeDefinition resultType, params DataTypeDefinition[] argTypes)
            :base(name)
        {                        
            ResultType = resultType;
            ArgTypes = argTypes.ToArray();
        }
        
        public int Arity => ArgTypes.Length;
        public DataTypeDefinition GetArgumentType(int i) => ArgTypes[i];

        public List<DataTypeDefinition> GetArgumentTypes(bool includeOutput = false) =>
            (!includeOutput ? ArgTypes : ArgTypes.Concat(new List<DataTypeDefinition> { ResultType })).ToList();

        public override bool Equals(object obj)
        {
            return obj is FunctionDefinition definition &&
                   Name == definition.Name &&
                   EqualityComparer<DataTypeDefinition>.Default.Equals(ResultType, definition.ResultType) &&
                   EqualityComparer<DataTypeDefinition[]>.Default.Equals(ArgTypes, definition.ArgTypes) &&
                   Arity == definition.Arity;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 685258752;
                hashCode += hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
                hashCode += hashCode * -1521134295 + new ArraySequenceEqualityComparer<DataTypeDefinition>().GetHashCode(ArgTypes);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{Name}({string.Join(", ", ArgTypes.ToList())}) -> {ResultType}";
        }
    }
}
