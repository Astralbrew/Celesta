using Astralbrew.Celesta.Script.Compile;
using Astralbrew.Celesta.Script.InterpretedEntities;
using Astralbrew.Celesta.Types;
using System;

namespace Astralbrew.Celesta.Script.CodePieces
{
    internal class ConstantValue : ICodePiece
    {
        public string DataType { get; }
        object Value { get; }

        public T GetValue<T>() => (T)Convert.ChangeType(Value, typeof(T));

        public ConstantValue(ConstantToken<string> token)
        {
            DataType = "String";
            Value = token.Value;
        }

        public ConstantValue(ConstantToken<int> token)
        {
            DataType = "Integer";
            Value = token.Value;
        }

        public ConstantValue(ConstantToken<SignedFixed24> token)
        {
            DataType = "Decimal";
            Value = token.Value;
        }

        public static ConstantValue FromToken(InterpretedToken token)
        {
            if (!token.IsConstant)
            {
                throw new ArgumentException("Creating constant code piece from non-constant token");
            }
            else
            {
                if (token.Type == InterpretedTokenType.Integer)
                    return new ConstantValue(token as ConstantToken<int>);

                else if (token.Type == InterpretedTokenType.String)
                    return new ConstantValue(token as ConstantToken<string>);

                else if (token.Type == InterpretedTokenType.Decimal)
                    return new ConstantValue(token as ConstantToken<SignedFixed24>);

                else throw new NotImplementedException();
            }            
        }

        public CompileTimeType GetCompileTimeType(CompileTimeContext context)
        {
            return new CompileTimeType(DataType);
        }

        public override string ToString()
            => DataType == "String" ? $"{DataType}:\"{Value}\"" : $"{DataType}:{Value}";
    }
}
