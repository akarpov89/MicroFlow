using System;
using System.Runtime.Serialization;

namespace MicroFlow
{
#if PORTABLE
    [DataContract]
#else
    [Serializable]
#endif
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

#if !PORTABLE
        protected FlowValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
#endif

#if PORTABLE
        [DataMember]
#endif
        public ValidationResult ValidatonResult { get; internal set; }
    }
}