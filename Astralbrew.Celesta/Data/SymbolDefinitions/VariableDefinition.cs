using System.Collections.Generic;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class VariableDefinition : AbstractSymbolDefinition
    {
        public DataTypeDefinition Type { get; }        

        public VariableDefinition(string name, DataTypeDefinition dataType) : base(name)
        {
            Type = dataType;
        }

        public override bool Equals(object obj)
        {
            return obj is VariableDefinition definition &&
                   Name == definition.Name;
        }

        public override int GetHashCode()
        {
            int hashCode = -1540434333;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);            
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Name}:{Type}";
        }
    }
}