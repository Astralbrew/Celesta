using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Utils;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class LoopNode : AbstractNode
    {
        public LoopNode(ISyntaxTreeNode runningCondition, ISyntaxTreeNode loopLogic)
            : base(SyntaxTreeNodeType.Loop, LanguageDefinition.PrimitiveTypes.Void)
        {
            RunningCondition = runningCondition;
            LoopLogic = loopLogic;

            RunningCondition.Parent = this;
            LoopLogic.Parent = this;
        }
        public ISyntaxTreeNode RunningCondition { get; }
        public ISyntaxTreeNode LoopLogic { get; }

        public override string ToString()
        {
            return $"while({RunningCondition})\n{{\n{LoopLogic.ToString().Indent()}\n}}";
        }
    }
}
