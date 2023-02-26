using Astralbrew.Celesta.Script;
using Astralbrew.Celesta.Script.CodePieces;
using Astralbrew.Celesta.Script.InterpretedEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.Celesta
{
    public class CelestaInterpreter
    {
        private static string RemoveRedundantWhitespace(string input)
        {
            return string.Join(" ", new Regex(@"(?:([""])(.*?)(?<!\\)(?>\\\\)*\1|([^""\s]+))")
                .Matches(input)
                .Cast<Match>()
                .Select(m => m.Value));
        }

        private static List<string> SplitToTokens(string input)
        {
            return new Regex(@"(?:([""])(.*?)(?<!\\)(?>\\\\)*\1|([^""\s]+))")
                .Matches(RemoveRedundantWhitespace(input))
                .Cast<Match>()
                .Select(m => m.Value)
                .Where(s => s != "")
                .Select(s =>
                {
                    if (s[0] == '"') return new List<string> { s };
                    return Regex.Split(s, @"(\<\=)|(\>\=)|(\=\=)|(\!\=)|([\+\-*()\^\/;:,=\<\>])|(\d*\.\d*)").ToList();
                })
                .SelectMany(x => x)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        public InterpreterContext Context = new InterpreterContext();

        public List<InterpretedToken> GetTokens(string input)
            => SplitToTokens(input).Select(Context.GuessToken).ToList();

        public ICodePiece Parse(string input)
        {            
            var parser = new Parser(GetTokens(input), Context.CompileTimeContext);            
            parser.Parse();
            parser.Stack.ToList().ForEach(Console.WriteLine);

            return new CodePieceBuilder(Context.CompileTimeContext, parser.Stack).FetchCodePiece();            
        }        
    }
}
