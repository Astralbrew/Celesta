using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class Conditional : ICodePiece
    {
        public ICodePiece Condition { get; }
        public ICodePiece ThenBranch { get; }
        public ICodePiece ElseBranch { get; }

        public Conditional(ICodePiece condition, ICodePiece thenBranch, ICodePiece elseBranch = null)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context) => CompileTimeType.Void;        
    }
}
