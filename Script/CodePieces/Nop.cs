using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal sealed class Nop : ICodePiece
    {
        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return new CompileTimeType("Void");
        }
    }
}
