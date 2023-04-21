using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Data.SymbolDefinitions
{
    public class AbstractSymbolDefinition : ISymbolDefinition
    {
        public string Name { get; }

        public AbstractSymbolDefinition(string name)
        {
            Name = name;
        }
    }
}
