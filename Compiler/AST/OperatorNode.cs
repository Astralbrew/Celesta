﻿using Astralbrew.Celesta.Data.SymbolDefinitions;
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
            Operator = operatorDefinition;
            Param1 = param;                        
        }

        public OperatorNode(OperatorDefinition operatorDefinition, ISyntaxTreeNode param1, ISyntaxTreeNode param2) 
            : base(SyntaxTreeNodeType.Operator, operatorDefinition.ResultType)
        {
            Operator = operatorDefinition;
            Param1 = param1;
            Param2 = param2;            
        }

        public OperatorDefinition Operator { get; }        
        public ISyntaxTreeNode Param1 { get; }
        public ISyntaxTreeNode Param2 { get; }

        public override string ToString()
        {
            if (Operator.IsUnary)
                return $"({Operator.Name}{Param1})";
            return $"({Param1} {Operator.Name} {Param2})";
        }
    }
}
