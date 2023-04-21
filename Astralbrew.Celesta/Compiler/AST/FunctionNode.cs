using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Astralbrew.Celesta.Compiler.AST
{
    public class FunctionNode : AbstractNode
    {
        public FunctionNode(FunctionDefinition function, params ISyntaxTreeNode[] aruments)
            : base(SyntaxTreeNodeType.Function, function.ResultType)
        {
            Function = function;
            Arguments = aruments.ToArray();

            foreach(var a in Arguments)
            {
                a.Parent = this;
            }
        }

        public FunctionDefinition Function { get; }
        ISyntaxTreeNode[] Arguments;

        public ISyntaxTreeNode[] GetArguments() => Arguments.ToArray();

        public override string ToString()
        {
            return Function.Name + "(" + string.Join(", ", (IEnumerable<ISyntaxTreeNode>)Arguments) + ")";
        }
    }
}
