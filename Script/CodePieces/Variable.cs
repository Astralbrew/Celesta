using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class Variable : ICodePiece
    {
        public string Name { get; }

        public Variable(string name)
        {
            Name = name;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return context.GetVariableType(Name);
        }

        public override string ToString()
        {
            return $"Var:{Name}";
        }
    }
}
