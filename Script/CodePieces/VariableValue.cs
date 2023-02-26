using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class VariableValue : ICodePiece
    {
        public string Name { get; set; }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return context.GetVariableType(Name);
        }
    }
}
