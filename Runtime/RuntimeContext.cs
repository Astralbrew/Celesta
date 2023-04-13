using Astralbrew.Celesta.Compiler;
using Astralbrew.Celesta.Compiler.AST;
using Astralbrew.Celesta.Data;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Runtime.Modules;
using System.Collections.Generic;
using System.Linq;
using static Astralbrew.Celesta.Constants.LanguageDefinition;

namespace Astralbrew.Celesta.Runtime
{
    public class RuntimeContext : ImplementationsCollection
    {
        internal DefinitionContext DefinitionContext { get; } = new DefinitionContext();

        Dictionary<VariableDefinition, object> Variables = new Dictionary<VariableDefinition, object>();
        
        private object GetValueOf(VariableDefinition variable)
        {
            if (Variables.ContainsKey(variable))
                return Variables[variable];            
            Variables[variable] = variable.Type.DefaultValue;
            return Variables[variable];
        }

        private object GetValueOf(object obj, DataTypeDefinition dataType)
        {
            if (obj != null) return obj;
            return dataType.DefaultValue;
        }

        public override void RegisterOperator(OperatorDefinition definition, IRuntimeImplementation implementation)
        {
            var op = DefinitionContext.RegisterOperator(definition);
            Operators[op] = implementation;
        }        

        public override void RegisterOperator(string name, DataTypeDefinition argType1, DataTypeDefinition argType2, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var op = DefinitionContext.RegisterOperator(name, argType1.Name, argType2.Name, outputType.Name);
            Operators[op] = implementation;
        }

        public override void RegisterOperator(string name, DataTypeDefinition argType, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var op = DefinitionContext.RegisterOperator(name, argType.Name, outputType.Name);
            Operators[op] = implementation;
        }

        public override void RegisterFunction(string name, DataTypeDefinition[] argTypes, DataTypeDefinition outputType, IRuntimeImplementation implementation)
        {
            var fn = DefinitionContext.RegisterFunction(name, argTypes.Select(a => a.Name).Concat(new string[] { outputType.Name }).ToArray());
            Functions[fn] = implementation;                     
        }

        public override void RegisterFunction(FunctionDefinition definition, IRuntimeImplementation implementation)
        {
            var fn = DefinitionContext.RegisterFunction(definition);
            Functions[fn] = implementation;
        }

        public void ImportModule(Module module)
        {
            foreach(var op in module.Operators)            
                RegisterOperator(op.Definition, op.Implementation);

            foreach (var fn in module.Functions)
                RegisterFunction(fn.Definition, fn.Implementation);
        }

        public virtual object Evaluate(ISyntaxTreeNode syntaxTreeNode)
        {
            if(syntaxTreeNode.Type == SyntaxTreeNodeType.Block)
            {
                var block = syntaxTreeNode as BlockNode;
                object last = null;
                foreach (var child in block.GetNodes())
                {
                    last = GetValueOf(Evaluate(child), child.OutputType);
                }
                return last;
            }

            if(syntaxTreeNode.Type==SyntaxTreeNodeType.Variable)
            {
                var variable = syntaxTreeNode as VariableNode;
                return GetValueOf(variable.Variable);
            }

            if(syntaxTreeNode.Type==SyntaxTreeNodeType.Constant)
            {                
                var constant = syntaxTreeNode as ConstantNode;                
                if (constant.DataType == PrimitiveTypes.Integer)
                    return int.Parse(constant.Value);
                if (constant.DataType == PrimitiveTypes.Decimal)
                    return double.Parse(constant.Value);
                if (constant.DataType == PrimitiveTypes.String)
                    return constant.Value.Substring(1, constant.Value.Length - 2); // drop " "
                if (constant.DataType == PrimitiveTypes.Boolean) 
                    return constant.Value == "true";
                return null;
            }

            if(syntaxTreeNode.Type==SyntaxTreeNodeType.Function)
            {
                var fcall = syntaxTreeNode as FunctionNode;
                object[] args = fcall.GetArguments().Select(this.Evaluate).ToArray();
                return GetValueOf(Functions[fcall.Function].Execute(args), fcall.OutputType);
            }

            if(syntaxTreeNode.Type== SyntaxTreeNodeType.Operator)
            {
                var op = syntaxTreeNode as OperatorNode;

                if (op.Operator.IsUnary) 
                {
                    var param = Evaluate(op.Param1);
                    return Operators[op.Operator].Execute(new object[] { param });
                }
                else
                {
                    var param1 = Evaluate(op.Param1);
                    var param2 = Evaluate(op.Param2);
                    return Operators[op.Operator].Execute(new object[] { param1, param2 });
                }
            }

            if(syntaxTreeNode.Type == SyntaxTreeNodeType.Assignment)
            {                
                var assignment = syntaxTreeNode as AssignNode;
                var variable = assignment.Symbol;
                Variables[variable] = Evaluate(assignment.RightHandSide);
                return Variables[variable];
            }

            if(syntaxTreeNode.Type == SyntaxTreeNodeType.Conditional)
            {
                var node = syntaxTreeNode as ConditionalNode;

                if (node.Condition.OutputType != PrimitiveTypes.Boolean) 
                {
                    throw new RuntimeException("Conditional expression must be of boolean type");
                }
                var cond = Evaluate(node.Condition);
                if ((bool)cond == true)
                    return Evaluate(node.ThenBranch);
                else
                    return Evaluate(node.ElseBranch);                
            }

            if(syntaxTreeNode.Type==SyntaxTreeNodeType.Loop)
            {
                var node = syntaxTreeNode as LoopNode;

                if (node.RunningCondition.OutputType != PrimitiveTypes.Boolean) 
                {
                    throw new RuntimeException("Conditional expression must be of boolean type");
                }

                while ((bool)Evaluate(node.RunningCondition)) 
                {
                    Evaluate(node.LoopLogic);
                }
                return new NoOutput();
            }

            return null;
        }


        public static RuntimeContext DefaultRuntimeContext
        {
            get
            {
                var context = new RuntimeContext();

                var Int = PrimitiveTypes.Integer;
                var Str = PrimitiveTypes.String;
                var Bool = PrimitiveTypes.Boolean;

                context.RegisterOperator("-", Int, Int, a => -(int)a);
                context.RegisterOperator("+", Int, Int, a => (int)a);

                context.RegisterOperator("+", Int, Int, Int, (a, b) => (int)a + (int)b);
                context.RegisterOperator("-", Int, Int, Int, (a, b) => (int)a - (int)b);
                context.RegisterOperator("*", Int, Int, Int, (a, b) => (int)a * (int)b);
                context.RegisterOperator("/", Int, Int, Int, (a, b) => (int)a / (int)b);
                context.RegisterOperator("%", Int, Int, Int, (a, b) => (int)a % (int)b);

                context.RegisterOperator("==", Int, Int, Bool, (a, b) => (int)a == (int)b);
                context.RegisterOperator("!=", Int, Int, Bool, (a, b) => (int)a != (int)b);
                context.RegisterOperator("<", Int, Int, Bool, (a, b) => (int)a < (int)b);
                context.RegisterOperator("<=", Int, Int, Bool, (a, b) => (int)a <= (int)b);
                context.RegisterOperator(">", Int, Int, Bool, (a, b) => (int)a > (int)b);
                context.RegisterOperator(">=", Int, Int, Bool, (a, b) => (int)a >= (int)b);



                context.RegisterOperator("+", Str, Str, Str, (a, b) => (string)a + (string)b);
                context.RegisterOperator("*", Str, Int, Str, (a, b) => string.Concat(Enumerable.Repeat((string)a, (int)b)));

                //context.RegisterFunction("abs", new DataTypeDefinition[] { Int }, Int, x => Math.Abs((int)x));

                context.ImportModule(SystemModule.Module);
                context.ImportModule(MathModule.Module);

                return context;
            }
        }        
    }
}
