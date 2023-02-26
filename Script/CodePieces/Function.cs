using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class Function : ICodePiece
    {
        public string Name { get; }

        public ICodePiece[] Arguments { get; }

        public Function(string name, params ICodePiece[] arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return context.GetFunctionType(Name, Arguments.Select(a => a.GetCompileTimeType(context)).ToArray());
        }        
    }
}
