using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal interface IBlockNode : ISyntaxTreeNode
    {
        IEnumerable<ISyntaxTreeNode> GetNodes();
        int NodesCount { get; }
        ISyntaxTreeNode GetNode(int i);
    }
}
