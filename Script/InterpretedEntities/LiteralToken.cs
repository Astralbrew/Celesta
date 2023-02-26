using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class LiteralToken : InterpretedToken
    {
        public string Name { get; }
        internal LiteralToken(string name, InterpretedTokenType type = InterpretedTokenType.Symbol) : base(type)
        {
            if(!Regex.IsMatch(name, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
            {
                throw new ArgumentException("Literal token is invalid");
            }
            Name = name;
        }

        public override string ToString() => $"<{Type} {Name}>";
    }
}
