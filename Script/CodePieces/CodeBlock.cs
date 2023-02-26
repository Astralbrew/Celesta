using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class CodeBlock : ICodePiece
    {
        public ICodePiece[] Content { get; }

        public CodeBlock(ICodePiece[] content)
        {
            Content = content;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context) => CompileTimeType.Void;        
    }
}
