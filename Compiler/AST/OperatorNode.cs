using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler.AST
{
    internal class OperatorNode : AbstractNode
    {
        public OperatorNode(OperatorDefinition operatorDefinition, ISyntaxTreeNode param) 
            : base(SyntaxTreeNodeType.Operator, operatorDefinition.ResultType)
        {
            OperatorDefinition = operatorDefinition;
            Param1 = param;                        
        }

        public OperatorNode(OperatorDefinition operatorDefinition, ISyntaxTreeNode param1, ISyntaxTreeNode param2) 
            : base(SyntaxTreeNodeType.Operator, operatorDefinition.ResultType)
        {
            OperatorDefinition = operatorDefinition;
            Param1 = param1;
            Param2 = param2;            
        }

        public OperatorDefinition OperatorDefinition { get; }        
        public ISyntaxTreeNode Param1 { get; }
        public ISyntaxTreeNode Param2 { get; }

        public override string ToString()
        {
            if (OperatorDefinition.IsUnary)
                return $"({OperatorDefinition.Name}{Param1})";
            return $"({Param1} {OperatorDefinition.Name} {Param2})";
        }
    }
}
