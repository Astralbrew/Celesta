using Astralbrew.Celesta.Script.Compile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class BinaryExpression : ICodePiece
    {
        public string Operator { get; }
        public ICodePiece FirstMember { get; }
        public ICodePiece SecondMember { get; }        

        public BinaryExpression(string @operator, ICodePiece firstMember, ICodePiece secondMember)
        {
            Operator = @operator;
            FirstMember = firstMember;
            SecondMember = secondMember;            
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return context
                .GetOperatorType(Operator, FirstMember.GetCompileTimeType(context), SecondMember.GetCompileTimeType(context));
        }
    }
}
