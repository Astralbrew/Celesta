using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Utils;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class ConditionalNode : AbstractNode
    {
        public ConditionalNode(ISyntaxTreeNode condition, ISyntaxTreeNode thenBranch)
            : base(SyntaxTreeNodeType.Conditional, thenBranch.OutputType)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = null;
        }

        public ConditionalNode(ISyntaxTreeNode condition, ISyntaxTreeNode thenBranch, ISyntaxTreeNode elseBranch)
            : base(SyntaxTreeNodeType.Conditional,
                  thenBranch.OutputType == elseBranch.OutputType
                    ? thenBranch.OutputType
                    : LanguageDefinition.PrimitiveTypes.Void)
        {
            Condition = condition;
            ThenBranch = thenBranch;
            ElseBranch = elseBranch;
        }

        public ISyntaxTreeNode Condition { get; }
        public ISyntaxTreeNode ThenBranch { get; }
        public ISyntaxTreeNode ElseBranch { get; }

        public override string ToString()
        {
            if (ElseBranch != null)
                return $"if({Condition})\n{{\n{ThenBranch.ToString().Indent()}\n}}else{{\n{ElseBranch.ToString().Indent()}\n}}";
            else
                return $"if({Condition})\n{{\n{ThenBranch.ToString().Indent()}\n}}";
        }
    }
}
