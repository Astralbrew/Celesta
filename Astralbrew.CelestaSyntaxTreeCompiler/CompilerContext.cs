using Astralbrew.Celesta.Compiler.AST;
using Astralbrew.Celesta.Runtime;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Compiler;
using Astralbrew.CelestaSyntaxTreeCompiler.Assembler;
using Astralbrew.Celesta.Constants;
using static Astralbrew.Celesta.Constants.LanguageDefinition;

namespace Astralbrew.CelestaSyntaxTreeCompiler
{
    internal class CompilerContext : RuntimeContext
    {
        record struct BinaryOperatorSignature(string Operator, DataTypeDefinition Argument1Type, DataTypeDefinition Argument2Type);        
        record struct UnaryOperatorSignature(string Operator, DataTypeDefinition ArgumentType);

        Dictionary<OperatorDefinition, string> OperatorInstructions = new Dictionary<OperatorDefinition, string>();
        Dictionary<FunctionDefinition, int> SysCalls = new Dictionary<FunctionDefinition, int>();

        public void RegisterInstruction(string instrName, string @operator, DataTypeDefinition a1Type, DataTypeDefinition a2Type, DataTypeDefinition result)
        {
            var op = new OperatorDefinition(@operator, result, a1Type, a2Type);
            OperatorInstructions[op] = instrName;            
            RegisterOperator(op, null as IRuntimeImplementation);
        }

        public void RegisterInstruction(string instrName, string @operator, string a1Type, string a2Type, string resType)
        {
            RegisterInstruction(instrName, @operator
                , DefinitionContext.GetDataType(a1Type, throwOnNotFound: true)
                , DefinitionContext.GetDataType(a2Type, throwOnNotFound: true)
                , DefinitionContext.GetDataType(resType, throwOnNotFound: true));            
        }

        public void RegisterInstruction(string instrName, string @operator, DataTypeDefinition argType, DataTypeDefinition result)
        {
            var op = new OperatorDefinition(@operator, result, argType);
            OperatorInstructions[op] = instrName;
            RegisterOperator(op, null as IRuntimeImplementation);
        }

        public void RegisterInstruction(string instrName, string @operator, string argType, string resType)
        {
            RegisterInstruction(instrName, @operator
             , DefinitionContext.GetDataType(argType, throwOnNotFound: true)
             , DefinitionContext.GetDataType(resType, throwOnNotFound: true));
        }        

        public void RegisterSysCall(FunctionDefinition function)
        {
            if (SysCalls.ContainsKey(function))
                return;
            var k = SysCalls.Count;
            SysCalls[function] = k;
            RegisterFunction(function, null as IRuntimeImplementation);
        }

        private MemoryPool MemoryPool = new MemoryPool();

        int LabelCount = 0;

        private Label NewLabel() => new Label($"L{LabelCount++}");

        class _RomPool
        {
            Dictionary<string, RomSymbol> Symbols = new Dictionary<string, RomSymbol>();

            public int Offset { get; set; } = 0;

            public RomSymbol Get(string name, ConstantNode node)
            {
                if(!Symbols.ContainsKey(name))
                {
                    Symbols[name] = RomSymbol.Make(name, node);
                }
                return Symbols[name];
            }            

            public byte[] SolveOffsets()
            {
                using(var ms = new MemoryStream())
                {
                    using(var bw= new BinaryWriter(ms))
                    {
                        foreach(var s in Symbols.Values)
                        {
                            s.Offset = Offset;
                            bw.Write(s.Value);
                            for (int len = s.Value.Length; len % 4 != 0; len++) 
                            {
                                bw.Write((byte)0);                                
                            }
                            Offset += s.Size;
                        }
                    }
                    return ms.ToArray();
                }
            }
        }
        _RomPool RomPool;

        private List<IAssemblyItem> Assemble(ISyntaxTreeNode node)
        {
            if (node is VariableNode v)
            {
                var result = new List<IAssemblyItem> { new Instruction("push", new Symbol(v), null) };

                if (v.OutputType == PrimitiveTypes.String && !(v.Parent is FunctionNode)) 
                    result.Add(new Instruction("stack strcln"));
                return result;
            }

            if (node is ConstantNode c)
            {
                var result = new List<IAssemblyItem> { new Instruction("push", new Symbol(c), null) };

                if (c.OutputType == PrimitiveTypes.String && !(c.Parent is FunctionNode))
                    result.Add(new Instruction("stack strcln"));

                return result;                
            }                

            if(node is BlockNode block)
            {
                var result = new List<IAssemblyItem>();

                var items = block.GetNodes().Select(Assemble).SelectMany(l => l).ToList();

                var consts = MemoryPool[block].Where(r => r.IsReadOnly).ToList();                

                foreach (var cst in consts) 
                {
                    var nd = cst.Node as ConstantNode;
                    var romsym = RomPool.Get(cst.Name, nd);

                    if (nd.OutputType == PrimitiveTypes.String) 
                    {
                        result.Add(new Instruction("strld", new Symbol(cst.Name, true), romsym));

                    }
                    else
                    {
                        result.Add(new Instruction("ld", new Symbol(cst.Name, true), romsym));
                    }
                }

                result.AddRange(items);


                foreach (var cst in consts)
                {
                    var nd = cst.Node as ConstantNode;
                    var romsym = RomPool.Get(cst.Name, nd);

                    if (nd.OutputType == PrimitiveTypes.String)
                    {
                        result.Add(new Instruction("dealloc", new Symbol(cst.Name, true)));

                    }                   
                }

                var vars = MemoryPool[block].Where(r => !r.IsReadOnly).ToList();

                foreach (var vrb in vars)
                {
                    var nd = vrb.Node as VariableNode;

                    if (nd.OutputType == PrimitiveTypes.String)
                    {
                        result.Add(new Instruction("dealloc", new Symbol(vrb.Name, false)));

                    }
                }

                return result;
            }

            if (node is OperatorNode op) 
            {
                var result = new List<IAssemblyItem>();

                var oper = op.Operator;                
             
                result.AddRange(Assemble(op.Param1));                

                if(!oper.IsUnary)
                {                    
                    result.AddRange(Assemble(op.Param2));
                }

                result.Add(new Instruction($"stack {OperatorInstructions[oper]}"));

                return result;
            }

            if(node is AssignNode assign)
            {
                var result = new List<IAssemblyItem>();

                result.AddRange(Assemble(assign.RightHandSide));

                var lhs = assign.LeftHandSide;

                if(lhs is VariableNode av)
                {
                    if (assign.IsInExpression)
                        result.Add(new Instruction("peek", new Symbol(av)));
                    else
                    {
                        if(av.OutputType == PrimitiveTypes.String)
                            result.Add(new Instruction("pop strfix", new Symbol(av)));
                        else
                            result.Add(new Instruction("pop", new Symbol(av)));
                    }
                }
                else
                {
                    throw new ArgumentException($"Invalid left hand side : {lhs.OutputType}");
                }
                return result;
            }

            if (node is ConditionalNode cond)
            {
                var falseLabel = NewLabel();
                var endLabel = NewLabel();

                var result = new List<IAssemblyItem>();
                result.AddRange(Assemble(cond.Condition));
                result.Add(new Instruction("pop test"));
                result.Add(new Instruction("jf", falseLabel));
                result.AddRange(Assemble(cond.ThenBranch));
                result.Add(new Instruction("jmp", endLabel));
                result.Add(falseLabel);
                if (cond.ElseBranch != null)
                {
                    result.AddRange(Assemble(cond.ElseBranch));                    
                }
                result.Add(endLabel);
                return result;
            }
            if(node is LoopNode loop)
            {
                var startLabel = NewLabel();
                var endLabel = NewLabel();

                var result = new List<IAssemblyItem>();
                result.Add(startLabel);

                result.AddRange(Assemble(loop.RunningCondition));
                result.Add(new Instruction("pop test"));
                result.Add(new Instruction("jf", endLabel));
                result.AddRange(Assemble(loop.LoopLogic));
                result.Add(new Instruction("jmp", startLabel));
                result.Add(endLabel);

                return result;
            }
            if(node is RepeatNode rept)
            {
                var startLabel = NewLabel();
                var endLabel = NewLabel();

                var result = new List<IAssemblyItem>();
                result.AddRange(Assemble(rept.NumberOfIterations));
                result.Add(startLabel);
                result.Add(new Instruction("stack dec"));
                result.Add(new Instruction("peek test zero"));                
                result.Add(new Instruction("jt", endLabel));
                result.AddRange(Assemble(rept.LoopLogic));
                result.Add(new Instruction("jmp", startLabel));
                result.Add(endLabel);

                return result;
            }
            if (node is FunctionNode func) 
            {
                var result = new List<IAssemblyItem>();
                result.AddRange(func.GetArguments().Select(Assemble).SelectMany(l => l));
                result.Add(new Instruction("syscall", new Literal(SysCalls[func.Function])));
                return result;
            }


            return new List<IAssemblyItem>();
        }

        public override object Evaluate(ISyntaxTreeNode syntaxTreeNode)
        {            
            syntaxTreeNode.Scan<VariableNode>(MemoryPool.AddRecord);
            syntaxTreeNode.Scan<ConstantNode>(MemoryPool.AddRecord);
            //MemoryPool.AddRecord(new ConstantNode("0"));
            //MemoryPool.AddRecord(new ConstantNode("1"));
            MemoryPool.ResolveOffsets();
            LabelCount = 0;

            RomPool = new _RomPool();

            var assembleList = Assemble(syntaxTreeNode);
            assembleList.Add(new Instruction("exit"));            

            int ramSize = 4096;
            int heapStart = MemoryPool.HeapStartOffset;
            Console.WriteLine(4 * assembleList.Where(i => i is Instruction).Count());
            int assemblySize = 4 * assembleList.Where(i => i is Instruction).Count();

            for (int i = 0, labelsCount = 0; i < assembleList.Count; i++) 
            {
                if(assembleList[i] is Label label)
                {
                    label.Offset = 4 * (i - labelsCount);
                    labelsCount++;
                }
            }


            assembleList = assembleList.RemoveDuplicateLabels();
            // remove duplicate labels
            for (int i = 0; i < assembleList.Count; i++)
            {
                if (assembleList[i] is Label label)
                {
                    var sameLabels = assembleList.Skip(i + 1).Where(x => (x is Label L) && (L.Offset == label.Offset));
                    foreach (var l in sameLabels)
                    {
                        foreach(Instruction ins in assembleList.Where(x=>x is Instruction))
                        {
                            if (ins.Operand1 == l) ins.Operand1 = label;
                            if (ins.Operand2 == l) ins.Operand2 = label;
                        }
                    }
                }
            }

            assembleList = assembleList.Where(i => !(i is Label) || (i is Label
                && assembleList.Any(x => (x is Instruction instr) && (instr.Operand1 == i || instr.Operand2 == i))))
                .ToList();

            RomPool.Offset = assemblySize;

            var rom = RomPool.SolveOffsets();

            foreach(var i in assembleList)
            {
                if(i is Instruction instr)
                {
                    if(instr.Operand1 is Symbol sym1)
                    {
                        sym1.Offset = MemoryPool[sym1.Name].Offset;
                    }
                }
            }

            var code = assembleList.Where(i => i is Instruction).Select(i => (i as Instruction).GetCode()).ToArray();

            using(var ms=new MemoryStream())
            {
                using(var bw= new BinaryWriter(ms))
                {
                    bw.Write(ramSize);
                    bw.Write(heapStart);
                    bw.Write(code.ToBytes());
                    bw.Write(rom);
                }
                return new AssembledCode(assembleList, ms.ToArray());
            }                       
        }       

        public static CompilerContext TestCompiler
        {
            get
            {
                var ctx = new CompilerContext();

                ctx.RegisterInstruction("add", "+", "int", "int", "int");
                ctx.RegisterInstruction("sub", "-", "int", "int", "int");
                ctx.RegisterInstruction("mul", "*", "int", "int", "int");
                ctx.RegisterInstruction("div", "/", "int", "int", "int");
                ctx.RegisterInstruction("mod", "%", "int", "int", "int");

                ctx.RegisterInstruction("eq", "==", "int", "int", "bool");
                ctx.RegisterInstruction("neq", "!=", "int", "int", "bool");

                ctx.RegisterInstruction("lt", "<", "int", "int", "bool");
                ctx.RegisterInstruction("le", "<=", "int", "int", "bool");
                ctx.RegisterInstruction("gt", ">", "int", "int", "bool");
                ctx.RegisterInstruction("ge", ">=", "int", "int", "bool");

                ctx.RegisterInstruction("and", "and", "bool", "bool", "bool");
                ctx.RegisterInstruction("or", "or", "bool", "bool", "bool");

                ctx.RegisterInstruction("strcat", "+", "string", "string", "string");

                ctx.RegisterSysCall(new FunctionDefinition("print", PrimitiveTypes.Void, PrimitiveTypes.String));
                ctx.RegisterSysCall(new FunctionDefinition("randint", PrimitiveTypes.Integer));

                return ctx;
            }
        }
    }

    static class IAssemblyItemListExtensions
    {
        public static List<IAssemblyItem> RemoveDuplicateLabels(this List<IAssemblyItem> items)
        {
            var assembleList = items.ToList();
            for (int i = 0; i < assembleList.Count; i++)
            {
                if (assembleList[i] is Label label)
                {
                    var sameLabels = assembleList.Skip(i + 1).Where(x => (x is Label L) && (L.Offset == label.Offset));
                    foreach (var l in sameLabels)
                    {
                        foreach (Instruction ins in assembleList.Where(x => x is Instruction))
                        {
                            if (ins.Operand1 == l) ins.Operand1 = label;
                            if (ins.Operand2 == l) ins.Operand2 = label;
                        }
                    }
                }
            }

            return assembleList.Where(i => !(i is Label) || (i is Label
                && assembleList.Any(x => (x is Instruction instr) && (instr.Operand1 == i || instr.Operand2 == i))))
                .ToList();
        }
    }
}
