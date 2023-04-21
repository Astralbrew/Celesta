using Astralbrew.Celesta.Compiler;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;

namespace Astralbrew.Celesta.Runtime
{
    public class Interpreter
    {
        private Parser Parser = new Parser();
        public RuntimeContext RuntimeContext;
        private SymbolSolver SymbolSolver;

        public Interpreter(RuntimeContext runtimeContext = null)
        {
            RuntimeContext = runtimeContext ?? new RuntimeContext();
            SymbolSolver = new SymbolSolver(RuntimeContext.DefinitionContext);
        }

        public object Evaluate(string input)
        {
            var parseTree = Parser.Parse(input);
            var syntaxTree = SymbolSolver.ToSyntaxTreeNode(parseTree);
            //Console.WriteLine(syntaxTree);
            return RuntimeContext.Evaluate(syntaxTree);
        }
    }
}
