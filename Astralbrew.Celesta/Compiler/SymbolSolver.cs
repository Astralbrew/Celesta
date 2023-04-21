using Astralbrew.Celesta.Compiler.AST;
using Astralbrew.Celesta.Constants;
using Astralbrew.Celesta.Data.SymbolDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Compiler
{
    public class SymbolSolver
    {
        public DefinitionContext Context;

        public SymbolSolver(DefinitionContext context)
        {
            Context = context;
        }

        public ISyntaxTreeNode ToSyntaxTreeNode(ParseTreeNode parseTreeNode)
        {
            if (parseTreeNode == null) return null;
            var label = parseTreeNode.Label;
            if (parseTreeNode.IsTerminal)
            {
                if (parseTreeNode.Label == "~scope")
                    return new BlockNode(parseTreeNode.ScopeName);
                if (Parser.IsSymbol(label))
                    return new VariableNode(Context.GetVariable(label, parseTreeNode.ScopeName, true));
                else
                    return new ConstantNode(label);
            }
            if(LanguageDefinition.Operators.Contains(label))
            {
                var member1 = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                var member2 = ToSyntaxTreeNode(parseTreeNode.Children[1]);
                var oper = Context.GetOperator(label, member1.OutputType, member2.OutputType, true);
                return new OperatorNode(oper, member1, member2); ; 
            }
            if(label=="+u" || label=="-u")
            {
                var member = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                var oper = Context.GetOperator(label[0].ToString(), member.OutputType, true);
                return new OperatorNode(oper, member);
            }
            if(label=="~decl")
            {
                if (parseTreeNode.Children.Length == 2)
                {
                    string type = parseTreeNode.Children[0].Label;
                    string name = parseTreeNode.Children[1].Label + parseTreeNode.ScopeName;
                    Context.RegisterVariable(name, type);
                    DataTypeDefinition datatype = Context.GetDataType(type, true);
                    var variable = new VariableNode(Context.GetVariable(name, true));
                    return new AssignNode(variable, new ConstantNode(datatype.DefaultValueSer), true);
                }
                else if (parseTreeNode.Children.Length == 3)
                {
                    string type = parseTreeNode.Children[0].Label;
                    string name = parseTreeNode.Children[1].Label + parseTreeNode.ScopeName;
                    var value = parseTreeNode.Children[2];
                    Context.RegisterVariable(name, type);
                    var variable = new VariableNode(Context.GetVariable(name, true));
                    return new AssignNode(variable, ToSyntaxTreeNode(value), isDeclaration: true);

                }
                else throw new ArgumentException("Invalid syntax tree");
            }
            if(label=="~assign")
            {
                var v = Context.GetVariable(parseTreeNode.Children[0].Label, parseTreeNode.ScopeName, true);
                var rhs = ToSyntaxTreeNode(parseTreeNode.Children[1]);

                if(rhs.OutputType != v.Type)
                {
                    throw new ArgumentException("Assignment type mismatch");
                }

                return new AssignNode(new VariableNode(v), rhs);
            }
            if(label=="~scope")
            {                
                var instrs = new List<ISyntaxTreeNode>();
                foreach(var child in parseTreeNode.Children)
                {
                    instrs.Add(ToSyntaxTreeNode(child));
                }
                return new BlockNode(parseTreeNode.ScopeName, instrs.ToArray());
            }
            if(label=="~if")
            {
                if (parseTreeNode.Children.Length == 2)
                {
                    var _cond = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                    var _then = ToSyntaxTreeNode(parseTreeNode.Children[1]);
                    return new ConditionalNode(_cond, _then);
                }
                else if (parseTreeNode.Children.Length == 3) 
                {
                    var _cond = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                    var _then = ToSyntaxTreeNode(parseTreeNode.Children[1]);
                    var _else = ToSyntaxTreeNode(parseTreeNode.Children[2]);
                    return new ConditionalNode(_cond, _then, _else);
                }
                else throw new ArgumentException("Invalid syntax tree");
            }
            if(label=="~while")
            {
                var _cond = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                var _loop = ToSyntaxTreeNode(parseTreeNode.Children[1]);
                return new LoopNode(_cond, _loop);
            }
            if(label=="~repeat")
            {
                var _noIterations = ToSyntaxTreeNode(parseTreeNode.Children[0]);
                var _loop = ToSyntaxTreeNode(parseTreeNode.Children[1]);
                return new RepeatNode(_noIterations, _loop);
            }
            if(label=="~fun")
            {
                var args = parseTreeNode.Children.Skip(1).Select(t => ToSyntaxTreeNode(t));
                var datatypes = args.Select(st => st.OutputType).ToArray();
                var func = Context.GetFunction(parseTreeNode.Children[0].Label, datatypes, true);
                return new FunctionNode(func, args.ToArray());
            }

            return null;
        }
    }
}
