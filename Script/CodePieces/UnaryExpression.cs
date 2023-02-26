using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class UnaryExpression
    {
        public string Operator { get; }
        public ICodePiece Member { get; }

        public UnaryExpression(string @operator, ICodePiece member)
        {
            Operator = @operator;
            Member = member;
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return context.GetOperatorType(Operator, Member.GetCompileTimeType(context));
        }
    }
}
