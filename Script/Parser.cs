using Astralbrew.Celesta.Script.CodePieces;
using Astralbrew.Celesta.Script.Compile;
using Astralbrew.Celesta.Script.InterpretedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script
{
    internal class Parser
    {
        public List<InterpretedToken> Tokens = new List<InterpretedToken>();

        CompileTimeContext Context;

        public static List<InterpretedToken> AddSeparators(List<InterpretedToken> tokens)
        {
            List<InterpretedToken> tk = new List<InterpretedToken>();

            if (tokens.Count == 0)
                return tk;

            tk.Add(tokens[0]);

            for(int i=1;i<tokens.Count;i++)
            {
                var left = tk.Last();
                var right = tokens[i];

                if ((left.IsRightParen || left.IsSymbol || left.IsConstant || left.IsKeywordNamed("endif"))
                    &&
                    (right.IsSymbol || right.IsConstant || right.IsKeywordNamed("if")
                        || right.IsKeywordNamed("assign")
                        || right.IsKeywordNamed("id")
                        )) 
                {
                    tk.Add(InterpretedToken.InstructionSeparator);
                }
                /*else if(left.IsKeywordNamed("then","else") && right.IsKeyword)
                {
                    tk.Add(InterpretedToken.InstructionSeparator);
                }*/
                tk.Add(tokens[i]);
            }

            return tk;
        }

        public Parser(List<InterpretedToken> tokens, CompileTimeContext context)
        {
            Context = context;
            Tokens = AddSeparators(tokens.Where(t => !t.IsSemicolon).ToList());
        }

        int Priority(InterpretedToken token)
        {
            if (token.IsKeywordNamed("endif")) return 2;
            if (token.IsInstructionSeparator) return 2;

            if (token.IsKeywordNamed("then","else", "if")) return 1;            

            if (token.IsKeyword) return 10;

            if (token.IsOperatorSymbol("=")) return 20;
            if (token.IsOperatorSymbol("or")) return 30;
            if (token.IsOperatorSymbol("and")) return 40;
            if (token.IsOperatorSymbol("not")) return 50;
            if (token.IsOperatorSymbol("<", ">", "<=", ">=", "==", "!=")) return 60;

            if (token.IsOperatorSymbol("+", "-")) return 70;
            if (token.IsOperatorSymbol("*", "/", "%")) return 80;

            if (token.IsUnaryOperatorSymbol("+", "-")) return 90;

            if (token.IsPeriod) return 100;

            if(token.IsSymbol)
            {
                if (Context.IsFunction((token as LiteralToken).Name))
                    return 110;
            }
            return -1;
        }

        PrecedenceType Precedence(InterpretedToken token)
        {            
            return PrecedenceType.Left;
        }

        public void Parse()
        {            
            foreach(var token in Tokens)
            {
                Console.WriteLine($"\n--- Parsing {token} ---");
                NextToken(token);

                Console.WriteLine("\n Stack: ");
                Stack.ToList().ForEach(Console.WriteLine);

                Console.WriteLine("\n Op: ");
                OperatorStack.ToList().ForEach(Console.WriteLine);

            }
            Console.WriteLine("\n\n");
            MoveAllFromOperatorStack();
        }       

        Func<InterpretedToken, bool> MeetLeftParen = tk => tk.IsLeftParen;               

        void MoveAllFromOperatorStack()
        {
            while (OperatorStack.Count > 0)
            {
                Stack.Push(OperatorStack.Pop());
            }

        }

        bool MoveFromOperatorStackUntil(Func<InterpretedToken, bool> pred, bool push_match = false, bool allow_empty = false)
        {
            bool ok = false;
            while(OperatorStack.Count>0)
            {
                var top = OperatorStack.Pop();
                if(pred(top))
                {
                    ok = true;
                    if (push_match) Stack.Push(top);
                    break;
                }
                Stack.Push(top);
            }
            if (ok) return true;
            return allow_empty;            
        }

        void PushToStack(InterpretedToken token)
        {
            //Console.WriteLine($"Pushed {token}");
            if (token.IsComma)
                return;
            Stack.Push(token);
        }

        void NextToken(InterpretedToken token)
        {            
            if(token.IsLeftParen)
            {
                OperatorStack.Push(token);
                return;
            }

            if(token.IsRightParen)
            {
                if (!MoveFromOperatorStackUntil(MeetLeftParen))
                    throw new ArgumentException("Parenthesis mismatch");

                return;
            }

            var priority = Priority(token);

            if (priority < 0)
            {
                PushToStack(token);
            }
            else
                PushToOperatorStack(token, priority);

        }

        void PushToOperatorStack(InterpretedToken token, int priority)
        {
            if(OperatorStack.Count > 0)
            {
                if (token.IsInstructionSeparator && OperatorStack.Peek().IsKeywordNamed("then", "else"))
                {
                    OperatorStack.Push(token);
                    return;
                }

                if (token.IsKeywordNamed("if") && OperatorStack.Peek().IsInstructionSeparator)
                {
                    OperatorStack.Push(token);
                    return;
                }

                if (token.IsKeywordNamed("if") && OperatorStack.Peek().IsKeywordNamed("then", "else")) 
                {
                    OperatorStack.Push(token);
                    return;
                }
            }

            if (token.IsKeywordNamed("endif"))
            {
                while (OperatorStack.Count > 0 && !OperatorStack.Peek().IsKeywordNamed("then", "else")) 
                {
                    PushToStack(OperatorStack.Pop());
                }
                if (OperatorStack.Count == 0)
                    throw new ArgumentException("Error processing if statement");
                PushToStack(OperatorStack.Pop());

                OperatorStack.Push(token);
                return;
            }

            if (token.IsKeywordNamed("else"))
            {
                while (OperatorStack.Count > 0 && !OperatorStack.Peek().IsKeywordNamed("then")) 
                {
                    PushToStack(OperatorStack.Pop());
                }
                if (OperatorStack.Count == 0)
                    throw new ArgumentException("Error processing if statement");
                PushToStack(OperatorStack.Pop());

                OperatorStack.Push(token);
                return;
            }

            if (token.IsKeywordNamed("then"))
            {
                while (OperatorStack.Count > 0 && !OperatorStack.Peek().IsKeywordNamed("if")) 
                {
                    PushToStack(OperatorStack.Pop());
                }
                if (OperatorStack.Count == 0)
                    throw new ArgumentException("Error processing if statement");
                PushToStack(OperatorStack.Pop());

                OperatorStack.Push(token);
                return;
            }

            while (OperatorStack.Count > 0 && priority <= Priority(OperatorStack.Peek())) 
            {
                if (priority == Priority(OperatorStack.Peek()))
                {                                        
                    if (Precedence(token) == PrecedenceType.Right) 
                        break;                    
                }
                PushToStack(OperatorStack.Pop());
            }
            OperatorStack.Push(token);
        }


        public Stack<InterpretedToken> Stack = new Stack<InterpretedToken>();
        public Stack<InterpretedToken> OperatorStack = new Stack<InterpretedToken>();
    }
}
