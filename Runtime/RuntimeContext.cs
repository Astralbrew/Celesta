using Astralbrew.Celesta.Compiler;
using Astralbrew.Celesta.Compiler.AST;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using Astralbrew.Celesta.Runtime.Implementation.Operators;
using Astralbrew.Celesta.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Astralbrew.Celesta.Constants.LanguageDefinition;

namespace Astralbrew.Celesta.Runtime
{
    public class RuntimeContext
    {
        internal DefinitionContext DefinitionContext { get; } = new DefinitionContext();

        Dictionary<VariableDefinition, object> Variables = new Dictionary<VariableDefinition, object>();

        Dictionary<FunctionDefinition, IRuntimeImplementation> Functions = new Dictionary<FunctionDefinition, IRuntimeImplementation>();

        Dictionary<OperatorDefinition, IRuntimeImplementation> Operators = new Dictionary<OperatorDefinition, IRuntimeImplementation>();

        private object GetValueOf(VariableDefinition variable)
        {
            if (Variables.ContainsKey(variable))
                return Variables[variable];            
            Variables[variable] = variable.Type.DefaultValue;
            return Variables[variable];
        }

        public void RegisterOperator(string name, string argType1, string argType2, string outputType, IRuntimeImplementation implementation)
        {
            var op = DefinitionContext.RegisterOperator(name, argType1, argType2, outputType);
            Operators[op] = implementation;            
        }        

        public virtual object Evaluate(ISyntaxTreeNode syntaxTreeNode)
        {
            if(syntaxTreeNode.Type == SyntaxTreeNodeType.Block)
            {
                var block = syntaxTreeNode as BlockNode;
                object last = null;
                foreach (var child in block.GetNodes())
                {
                    last = Evaluate(child);
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
                return Functions[fcall.Function].Execute(args);
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
            return null;
        }


        public static RuntimeContext DefaultRuntimeContext
        {
            get
            {
                var context = new RuntimeContext();

                context.RegisterOperator("+", "int", "int", "int", new BinaryOperator(o => (int)o[0] + (int)o[1]));                
                context.RegisterOperator("-", "int", "int", "int", new BinaryOperator(o => (int)o[0] - (int)o[1]));                
                context.RegisterOperator("*", "int", "int", "int", new BinaryOperator(o => (int)o[0] * (int)o[1]));
                context.RegisterOperator("/", "int", "int", "int", new BinaryOperator(o => (int)o[0] / (int)o[1]));

                context.RegisterOperator("+", "string", "string", "string", new BinaryOperator(o => (string)o[0] + (string)o[1]));
                context.RegisterOperator("*", "string", "int", "string", new BinaryOperator(o => string.Concat(Enumerable.Repeat((string)o[0], (int)o[1]))));

                return context;
            }
        }
    }
}
