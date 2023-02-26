using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal interface IInterpretedToken
    {
        /// <summary>
        /// Parsed token type
        /// </summary>
        InterpretedTokenType Type { get; }
    }
}
