using System;

namespace MicroFlow
{
    public sealed class NullLogger : ILogger
    {
        public LogLevel Verbosity { get; set; }

        public void Write(LogLevel level, string message)
        {
        }

        public void Write(LogLevel level, string format, object arg0)
        {
        }

        public void Write(LogLevel level, string format, object arg0, object arg1)
        {
        }

        public void Write(LogLevel level, string format, object arg0, object arg1, object arg2)
        {
        }

        public void Write(LogLevel level, string format, params object[] args)
        {
        }

        public void Exception(Exception exception)
        {
        }

        public void Exception(string message, Exception exception)
        {
        }
    }
}