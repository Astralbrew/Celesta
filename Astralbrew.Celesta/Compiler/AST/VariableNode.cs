using Astralbrew.Celesta.Data.SymbolDefinitions;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class VariableNode : AbstractNode
    {
        public VariableNode(VariableDefinition variable) : base(SyntaxTreeNodeType.Variable, variable.Type)
        {
            Variable = variable;            
        }

        public VariableDefinition Variable { get; }

        public override string ToString() => Variable.Name;

        public string GetScope()
        {
            ISyntaxTreeNode node = this;
            while (node != null && !(node is BlockNode))
            {
                node = node.Parent;
            }

            if (node == null) return "";
            return (node as BlockNode).ScopeName;
        }
    }
}
