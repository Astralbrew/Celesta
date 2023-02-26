using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astralbrew.Celesta.Script.InterpretedEntities
{
    public class InterpretedToken : IInterpretedToken
    {
        public InterpretedTokenType Type { get; }

        internal InterpretedToken(InterpretedTokenType type)
        {
            Type = type;
        }

        public override string ToString() => $"<{Type} Token>";

        public bool IsOperator => Type == InterpretedTokenType.Operator;
        public bool IsLeftParen => Type == InterpretedTokenType.LeftParen;
        public bool IsRightParen => Type == InterpretedTokenType.RightParen;
        public bool IsConstant => Type == InterpretedTokenType.Integer
            || Type == InterpretedTokenType.String || Type == InterpretedTokenType.Decimal;

        public bool IsKeyword => Type == InterpretedTokenType.Keyword;
        public bool IsComma => Type == InterpretedTokenType.Comma;
        public bool IsSymbol => Type == InterpretedTokenType.Symbol;
        public bool IsPeriod => Type == InterpretedTokenType.MethodCallPeriod;
        public bool IsSemicolon => Type == InterpretedTokenType.Semicolon;
        public bool IsInstructionSeparator => Type == InterpretedTokenType.InstructionSeparator;
        public bool IsKeywordNamed(params string[] name) => IsKeyword && name.Contains((this as KeywordToken).Name);
        public bool IsOperatorSymbol(params string[] s) => IsOperator && s.Contains((this as OperatorToken).Symbol);
        public bool IsUnaryOperator => IsOperator && (this as OperatorToken).IsUnary;
        public bool IsUnaryOperatorSymbol(params string[] s) 
            => IsUnaryOperator && s.Contains((this as OperatorToken).Symbol);

        public static InterpretedToken LeftParen => new InterpretedToken(InterpretedTokenType.LeftParen);
        public static InterpretedToken RightParen => new InterpretedToken(InterpretedTokenType.RightParen);
        public static InterpretedToken Period => new InterpretedToken(InterpretedTokenType.MethodCallPeriod);
        public static InterpretedToken Semicolon => new InterpretedToken(InterpretedTokenType.Semicolon);
        public static InterpretedToken Comma => new InterpretedToken(InterpretedTokenType.Comma);
        public static InterpretedToken InstructionSeparator => new InterpretedToken(InterpretedTokenType.InstructionSeparator);

        public static InterpretedToken Constant(string token_value)
        {
            try
            {
                return new StringToken(token_value);
            }
            catch(ArgumentException) { }

            try
            {
                return new IntegerToken(token_value);
            }
            catch (ArgumentException) { }

            try
            {
                return new DecimalToken(token_value);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Constant token is invalid");
            }
        }
        public static InterpretedToken Operator(string token_value) => new OperatorToken(token_value);
    }
}
