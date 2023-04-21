using Astralbrew.Celesta.Constants;
using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class BlockNode : AbstractNode, IBlockNode
    {
        private readonly ISyntaxTreeNode[] Nodes;

        public string ScopeName { get; }

        public BlockNode(string scopeName) : base(SyntaxTreeNodeType.Block, LanguageDefinition.PrimitiveTypes.Void)
        {
            ScopeName = scopeName;
            Nodes = new ISyntaxTreeNode[0];
        }


        public BlockNode(string scopeName, ISyntaxTreeNode[] nodes)
            : base(SyntaxTreeNodeType.Block, nodes.Length > 0 ? LanguageDefinition.PrimitiveTypes.Void : nodes.Last().OutputType)
        {
            ScopeName = scopeName;
            Nodes = nodes.ToArray();
            foreach (var n in nodes)
                n.Parent = this;
        }

        public int NodesCount => Nodes.Length;

        public ISyntaxTreeNode GetNode(int i) => Nodes[i];

        public IEnumerable<ISyntaxTreeNode> GetNodes() => Nodes.ToArray();

        public override string ToString()
        {
            return $"Scope@{ScopeName}\n" + string.Join("\n", (IEnumerable<ISyntaxTreeNode>)Nodes);
        }
    }
}
