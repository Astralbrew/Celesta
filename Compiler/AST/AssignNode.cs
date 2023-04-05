using Astralbrew.Celesta.Data.SymbolDefinitions;
using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class AssignNode : AbstractNode, ISyntaxTreeNode
    {
        public AssignNode(VariableDefinition symbol, ISyntaxTreeNode rhs) : base(SyntaxTreeNodeType.Assignment, symbol.Type)
        {
            Symbol = symbol;
            RightHandSide = rhs;
        }

        public VariableDefinition Symbol { get; }
        public ISyntaxTreeNode RightHandSide { get; }        

        public override string ToString() => $"{Symbol} = {RightHandSide}";
    }
}
