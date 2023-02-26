using Astralbrew.Celesta.Script;
using Astralbrew.Celesta.Script.Compile;
using Astralbrew.Celesta.Script.InterpretedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta
{
    public class InterpreterContext
    {
        public List<string> Keywords = new List<string>
            { "assign", "id", "if", "then", "else", "endif" };


        internal CompileTimeContext CompileTimeContext = new CompileTimeContext();

        public InterpretedToken GuessToken(string token)
        {
            if (token == ".") return InterpretedToken.Period;
            if (token == ",") return InterpretedToken.Comma;
            if (token == ";") return InterpretedToken.Semicolon;
            if (token == "(") return InterpretedToken.LeftParen;
            if (token == ")") return InterpretedToken.RightParen;

            if (Keywords.Contains(token))
                return new KeywordToken(token);

            try
            {
                return InterpretedToken.Operator(token);
            }
            catch(ArgumentException) { }

            try
            {
                return InterpretedToken.Constant(token);
            }
            catch (ArgumentException) { }

            return new LiteralToken(token);
        }

        public void RegisterDataType(string typeName) => CompileTimeContext.RegisterDataType(typeName);
        public void RegisterVariable(string varName, string typeName)
            => CompileTimeContext.RegisterVariable(varName, typeName);

        public void RegisterFunction(string funcName, params string[] typeNames)
        {
            var retType = typeNames.Last();
            var argTypes = typeNames.Take(typeNames.Length - 1).ToArray();
            CompileTimeContext.RegisterFunction(funcName, argTypes, retType);
        }        

        public IEnumerable<string> DumpFunctions() => CompileTimeContext.DumpFunctions();
    }
}
