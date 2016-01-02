using System;
using System.Runtime.Serialization;

namespace MicroFlow
{
    [Serializable]
    public class FlowValidationException : Exception
    {
        public FlowValidationException()
        {
        }

        public FlowValidationException(string message) : base(message)
        {
        }

        public FlowValidationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected FlowValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public ValidationResult ValidatonResult { get; internal set; }
    }
}