using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    internal class OperatorToken : InterpretedToken
    {
        private static readonly string[] Operators = new List<string>
        {
            "+", "-", "*", "/", "%", "=", "<", ">", "<=", ">=", "==", "!=",
            "or", "and", "not"
        }.ToArray();
        public string Symbol { get; }

        public bool IsUnary { get; private set; }

        internal OperatorToken(string symbol) : base(InterpretedTokenType.Operator)
        {
            if (!Operators.Contains(symbol)) 
            {
                throw new ArgumentException("Operator token is not valid");
            }
            Symbol = symbol;
        }

        public void MakeUnary() { IsUnary = true; }

        public override string ToString()
        {
            return $"<Operator '{Symbol}' {(IsUnary ? "Unary" : "")}>";
        }
        
    }
}
