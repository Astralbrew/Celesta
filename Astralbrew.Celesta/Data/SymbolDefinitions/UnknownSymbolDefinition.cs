using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class UnknownSymbolDefinition : AbstractSymbolDefinition
    {        
        public UnknownSymbolDefinition(string name) : base(name) { }

        public override bool Equals(object obj)
        {
            return obj is UnknownSymbolDefinition definition &&
                   Name == definition.Name;
        }

        public override int GetHashCode()
        {
            return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
        }
    }
}
