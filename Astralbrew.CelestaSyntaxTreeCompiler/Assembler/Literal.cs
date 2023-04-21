using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.CelestaSyntaxTreeCompiler.Assembler
{
    internal class Literal : ISymbol
    {
        public int Offset { get; }

        public Literal(int offset)
        {
            Offset = offset;
        }

        public override string ToString() => $"{Offset}";
    }
}
