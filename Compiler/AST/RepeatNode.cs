using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Utils;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class RepeatNode : AbstractNode
    {
        public RepeatNode(ISyntaxTreeNode numberOfIterations, ISyntaxTreeNode loopLogic) : base(SyntaxTreeNodeType.RepeatN, LanguageDefinition.PrimitiveTypes.Void)
        {
            NumberOfIterations = numberOfIterations;
            LoopLogic = loopLogic;

            NumberOfIterations.Parent = this;
            LoopLogic.Parent = this;
        }

        public ISyntaxTreeNode NumberOfIterations { get; }
        public ISyntaxTreeNode LoopLogic { get; }

        public override string ToString()
        {
            return $"repeat({NumberOfIterations})\n{{\n{LoopLogic.ToString().Indent()}\n}}";
        }

    }
}
