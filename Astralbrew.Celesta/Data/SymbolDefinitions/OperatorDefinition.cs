using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class OperatorDefinition : AbstractSymbolDefinition
    {        
        public OperatorDefinition(string name, DataTypeDefinition resultType, DataTypeDefinition argType1) : base(name)
        {
            ResultType = resultType;
            ArgType1 = argType1;            
            IsUnary = true;
        }

        public OperatorDefinition(string name, DataTypeDefinition resultType, DataTypeDefinition argType1, DataTypeDefinition argType2) : base(name)
        {
            ResultType = resultType;
            ArgType1 = argType1;
            ArgType2 = argType2;
            IsUnary = false;
        }

        public override string ToString()
        {
            return $"{Name}({ArgType1}{(IsUnary ? "" : $", {ArgType2}")}) -> {ResultType}";
        }

        public DataTypeDefinition ResultType { get; }
        public DataTypeDefinition ArgType1 { get; }
        public DataTypeDefinition ArgType2 { get; }
        public bool IsUnary { get; }

        public override bool Equals(object obj)
        {
            return obj is OperatorDefinition definition &&
                   Name == definition.Name &&
                   EqualityComparer<DataTypeDefinition>.Default.Equals(ArgType1, definition.ArgType1) &&
                   EqualityComparer<DataTypeDefinition>.Default.Equals(ArgType2, definition.ArgType2) &&
                   IsUnary == definition.IsUnary;
        }

        public override int GetHashCode()
        {
            int hashCode = -650863345;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<DataTypeDefinition>.Default.GetHashCode(ArgType1);
            hashCode = hashCode * -1521134295 + EqualityComparer<DataTypeDefinition>.Default.GetHashCode(ArgType2);
            hashCode = hashCode * -1521134295 + IsUnary.GetHashCode();
            return hashCode;
        }

        public List<DataTypeDefinition> GetArgumentTypes(bool includeOutput = false)
        {
            List<DataTypeDefinition> result = new List<DataTypeDefinition>();
            result.Add(ArgType1);
            if (!IsUnary)
                result.Add(ArgType2);
            if (includeOutput)
                result.Add(ResultType);
            return result;
        }

        public bool IsBinarySymbol(params string[] symbols) => !IsUnary && symbols.Contains(Name);
        public bool IsUnarySymbol(params string[] symbols) => IsUnary && symbols.Contains(Name);
        
    }
}
