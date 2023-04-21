using System;
using System.Runtime.Serialization;

namespace Astralbrew.Celesta.Compiler
{
    [Serializable]
    internal class DuplicateDefinitionException : Exception
    {
        public DuplicateDefinitionException()
        {
        }

        public DuplicateDefinitionException(string message) : base(message)
        {
        }

        public DuplicateDefinitionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}