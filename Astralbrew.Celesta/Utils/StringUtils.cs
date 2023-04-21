using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Utils
{
    public static class StringUtils
    {
        public static bool MatchesCStyleString(this string input)
        {
            return new Regex(@"^(?:([""])(.*?)(?<!\\)(?>\\\\)*\1)$").IsMatch(input);
        }

        private static string RemoveRedundantWhitespace(string input)
        {
            return string.Join(" ", new Regex(@"(?:([""])(.*?)(?<!\\)(?>\\\\)*\1|([^""\s]+))")
                .Matches(input)
                .Cast<Match>()
                .Select(m => m.Value));
        }

        private static string RemoveComments(string input)
        {
            var result = Regex.Replace(input, @"(\/\*.*?\*\/)", "", RegexOptions.Singleline);
            return Regex.Replace(result, @"\/\/([^\S\r\n]|.)*$", "", RegexOptions.Multiline);
        }

        public static List<string> SplitToTokens(this string input)
        {
            return new Regex(@"(?:([""])(.*?)(?<!\\)(?>\\\\)*\1|([^""\s]+))")
                .Matches(RemoveRedundantWhitespace(RemoveComments(input)))
                .Cast<Match>()
                .Select(m => m.Value)
                .Where(s => s != "")
                .Select(s =>
                {
                    if (s[0] == '"') return new List<string> { s };
                    return Regex.Split(s, @"(\<\=)|(\>\=)|(\=\=)|(\!\=)|([\+\-*\%()\^\/;:,=\<\>])|(\d*\.\d*)").ToList();
                })
                .SelectMany(x => x)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        public static string Indent(this string input)
        {
            return string.Join("\n", input.Split('\r', '\n').Select(line => "\t" + line));
        }
    }
}
