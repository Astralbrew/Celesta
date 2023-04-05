using Astralbrew.Celesta.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class BlockNode : AbstractNode, IBlockNode
    {
        private readonly ISyntaxTreeNode[] Nodes;

        public BlockNode() : base(SyntaxTreeNodeType.Block, LanguageDefinition.PrimitiveTypes.Void) 
        {
            Nodes = new ISyntaxTreeNode[0];
        }

        public BlockNode(ISyntaxTreeNode[] nodes)
            : base(SyntaxTreeNodeType.Block, nodes.Length > 0 ? LanguageDefinition.PrimitiveTypes.Void : nodes.Last().OutputType)
        {
            Nodes = nodes.ToArray();
        }

        public int NodesCount => Nodes.Length;

        public ISyntaxTreeNode GetNode(int i) => Nodes[i];

        public IEnumerable<ISyntaxTreeNode> GetNodes() => Nodes.ToArray();

        public override string ToString()
        {
            return string.Join("\n", (IEnumerable<ISyntaxTreeNode>)Nodes);
        }
    }
}
