﻿using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler.AST
{
    public abstract class AbstractNode : ISyntaxTreeNode
    {
        public ISyntaxTreeNode Parent { get; set; }
        public SyntaxTreeNodeType Type { get; }
        public DataTypeDefinition OutputType { get; }
        protected AbstractNode(SyntaxTreeNodeType type, DataTypeDefinition outputType)
        {
            Type = type;
            OutputType = outputType;
        }        
    }
}
