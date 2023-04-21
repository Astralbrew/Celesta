using System;
using System.Runtime.Serialization;

namespace Astralbrew.Celesta.Compiler
{
    [Serializable]
    internal class IdenitifierNotFoundException : Exception
    {
        public IdenitifierNotFoundException()
        {
        }

        public IdenitifierNotFoundException(string message) : base(message)
        {
        }

        public IdenitifierNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected IdenitifierNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}