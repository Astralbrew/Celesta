using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class KeywordToken : LiteralToken
    {
        internal KeywordToken(string name) : base(name, InterpretedTokenType.Keyword)
        { }
    }
}
