using Astralbrew.Celesta.Script.Compile;
using Astralbrew.Celesta.Script.InterpretedEntities;
using Astralbrew.Celesta.Types;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class ConstantValue : ICodePiece
    {
        public string DataType { get; }

        public ConstantValue(ConstantToken<string> token)
        {
            DataType = "String";
        }

        public ConstantValue(ConstantToken<int> token)
        {
            DataType = "Integer";
        }

        public ConstantValue(ConstantToken<SignedFixed24> token)
        {
            DataType = "Decimal";
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return new CompileTimeType(DataType);
        }
    }
}
