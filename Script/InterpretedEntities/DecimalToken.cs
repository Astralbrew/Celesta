using Astralbrew.Celesta.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class DecimalToken : ConstantToken<SignedFixed24>
    {
        public override SignedFixed24 Value { get; }

        /// <summary>
        /// Creates decimal token from decimal constant
        /// </summary>        
        public DecimalToken(string token_value) : base(InterpretedTokenType.Decimal)
        {
            if (SignedFixed24.TryParse(token_value, out SignedFixed24 result))
            {
                Value = result;
            }
            else
            {
                throw new ArgumentException("Decimal token is not valid");
            }
        }
    }
}
