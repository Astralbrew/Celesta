using Astralbrew.CelestaSyntaxTreeCompiler.Assembler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class AssembledCode
    {
        public List<IAssemblyItem> Items { get; }
        public byte[] Binary { get; }

        public AssembledCode(List<IAssemblyItem> items, byte[] binary)
        {
            Items = items;
            Binary = binary;
        }
    }
}
