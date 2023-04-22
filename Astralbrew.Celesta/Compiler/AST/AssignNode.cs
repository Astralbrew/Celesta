using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class AssignNode : AbstractNode, ISyntaxTreeNode
    {
        public AssignNode(ISyntaxTreeNode lhs, ISyntaxTreeNode rhs, bool isDeclaration = false)
            : base(SyntaxTreeNodeType.Assignment, lhs.OutputType)
        {
            LeftHandSide = lhs;
            RightHandSide = rhs;

            lhs.Parent = this;

            if (rhs != null)
                rhs.Parent = this;
            IsDeclaration = isDeclaration;
        }

        public bool IsDeclaration { get; }

        public ISyntaxTreeNode LeftHandSide { get; }        
        public ISyntaxTreeNode RightHandSide { get; }

        public bool IsInExpression => !(Parent is BlockNode);
        public override string ToString() => $"{LeftHandSide} = {RightHandSide}";
    }
}
