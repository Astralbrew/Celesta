using Astralbrew.Celesta.Compiler;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;

namespace Astralbrew.Celesta.Runtime
{
    internal class Interpreter
    {
        DefinitionContext DefinitionContext = new DefinitionContext();

        Dictionary<OperatorDefinition, Func<string[], string>> Operators;
        Dictionary<FunctionDefinition, Func<string[], string>> Functions;        
    }
}
