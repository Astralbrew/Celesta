using System;
using System.Runtime.Serialization;

namespace Astralbrew.Celesta.Compiler.AST
{
    [Serializable]
    internal class ConstantIdentificationException : Exception
    {
        public ConstantIdentificationException()
        {
        }

        public ConstantIdentificationException(string message) : base(message)
        {
        }

        public ConstantIdentificationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConstantIdentificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}