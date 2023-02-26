using Astralbrew.Celesta.Script.Compile;

namespace Astralbrew.Celesta.Script.CodePieces
{
    public interface ICodePiece
    {
        CompileTimeType GetCompileTimeType(CompileTimeContext context);
    }
}
