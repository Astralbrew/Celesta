using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class IntegerToken : ConstantToken<int>
    {
        public override int Value { get; }        

        /// <summary>
        /// Creates integer token from integer constant
        /// </summary>        
        public IntegerToken(string token_value) : base(InterpretedTokenType.Integer)
        {
            if (int.TryParse(token_value, out int result))
            {
                Value = result;
            }
            else
            {
                throw new ArgumentException("Integer token is not valid");
            }
        }
    }
}
