using Astralbrew.Celesta.Data.SymbolDefinitions;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class VariableNode : AbstractNode
    {
        public VariableNode(VariableDefinition variable) : base(SyntaxTreeNodeType.Variable, variable.Type)
        {
            Variable = variable;            
        }

        public VariableDefinition Variable { get; }

        public override string ToString() => Variable.Name;        
    }
}
