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

        public override string ToString()
        {
            string text = $"if({Condition}) then\n" +
                $"{string.Join("\n", ThenBranch.ToString().Split('\n').Select(s => "  " + s))}\n";
            string else_text = ElseBranch == null ? "" : "else\n" +
                $"{string.Join("\n", ElseBranch.ToString().Split('\n').Select(s => "  " + s))}\n";
            return text + else_text + "endif";
        }
    }
}
