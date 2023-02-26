using Astralbrew.Celesta.Script.CodePieces;
using Astralbrew.Celesta.Script.Compile;
using Astralbrew.Celesta.Script.InterpretedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script
{
    internal class CodePieceBuilder
    {
        public CompileTimeContext Context { get; }
        public Stack<InterpretedToken> Tokens { get; }

        public CodePieceBuilder(CompileTimeContext context, Stack<InterpretedToken> tokens)
        {
            Context = context;
            Tokens = tokens;
        }

        public ICodePiece FetchCodePiece()
        {
            if (Tokens.Count == 0)
                throw new ArgumentException("Invalid token sequence");

            var token = Tokens.Pop();
            if (token.IsConstant)
                return ConstantValue.FromToken(token);

            if(token.IsOperator)
            {
                string op = (token as OperatorToken).Symbol;
                if(token.IsUnaryOperator)
                {
                    return new UnaryExpression(op, FetchCodePiece());
                }
                else
                {
                    var second = FetchCodePiece();
                    var first = FetchCodePiece();
                    return new BinaryExpression(op, first, second);                    
                }
            }

            if(token.IsSymbol)
            {
                var name = (token as LiteralToken).Name;
                if(Context.IsFunction(name))
                {
                    List<ICodePiece> args = new List<ICodePiece>();
                    for (int i = 0; i < Context.GetFunctionArity(name); i++)
                        args.Add(FetchCodePiece());
                    args.Reverse();
                    return new Function(name, args.ToArray());
                }
                else if(Context.IsValidType(name))
                {
                    return new DataType(name);
                }
                else if(Context.IsVariable(name))
                {
                    return new Variable(name);
                }
            }

            if(token.IsInstructionSeparator)
            {
                ICodePiece p2 = FetchCodePiece();
                ICodePiece p1 = FetchCodePiece();
                var block = new CodeBlock(p1, p2);
                return block;
            }

            throw new NotImplementedException();
        }
    }
}
