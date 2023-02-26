using Astralbrew.Celesta.Script.Compile;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal interface ICodePiece
    {
        CompileTimeType GetCompileTimeType(CompileTimeContext context);
    }
}
