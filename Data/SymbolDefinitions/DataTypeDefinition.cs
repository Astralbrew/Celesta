using System.Collections.Generic;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class DataTypeDefinition : AbstractSymbolDefinition
    {
        public DataTypeDefinition(string name) : base(name) { }

        public override bool Equals(object obj)
        {
            return obj is DataTypeDefinition definition &&
                   Name == definition.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }

        public override string ToString()
        {
            return Name;
        }

        public static bool operator == (DataTypeDefinition d1, DataTypeDefinition d2) => Equals(d1, d2);
        public static bool operator !=(DataTypeDefinition d1, DataTypeDefinition d2) => !Equals(d1, d2);
    }
}
