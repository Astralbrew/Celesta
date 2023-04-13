using Astralbrew.Celesta.Data.SymbolDefinitions;

namespace Astralbrew.Celesta.Compiler.AST
{
    public interface ISyntaxTreeNode
    {
        SyntaxTreeNodeType Type { get; }
        DataTypeDefinition OutputType { get; }
        ISyntaxTreeNode Parent { get; set; }
    }
}
