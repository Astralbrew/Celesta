using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal abstract class ConstantToken<T> : InterpretedToken
    {
        internal ConstantToken(InterpretedTokenType type) : base(type) { }

        /// <summary>
        /// Value of the constant token
        /// </summary>
        public abstract T Value { get; }

        public override string ToString()
        {
            return $"<Constant {typeof(T).Name} Value=\"{Value}\">";
        }
    }
}
