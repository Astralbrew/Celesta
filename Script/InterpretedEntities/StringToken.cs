using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class StringToken : ConstantToken<string>
    {
        public override string Value { get; }        

        /// <summary>
        /// Creates string token from string constant
        /// </summary>
        /// <param name="token_value">string value surrounded by quotes e.g. "\"example\""</param>
        public StringToken(string token_value) : base(InterpretedTokenType.String)
        {
            if(!Regex.IsMatch(token_value, @"^([""])(.*?)(?<!\\)(?>\\\\)*\1$"))
            {
                throw new ArgumentException("String token is not valid");
            }
            Value = token_value.Substring(1, token_value.Length - 2);
        }
    }
}
