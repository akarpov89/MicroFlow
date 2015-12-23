using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public interface ILogger
    {
        LogLevel Verbosity { get; set; }

        void Write(LogLevel level, string message);

        [StringFormatMethod("format")]
        void Write(LogLevel level, string format, object arg0);

        [StringFormatMethod("format")]
        void Write(LogLevel level, string format, object arg0, object arg1);

        [StringFormatMethod("format")]
        void Write(LogLevel level, string format, object arg0, object arg1, object arg2);

        [StringFormatMethod("format")]
        void Write(LogLevel level, string format, params object[] args);

        void Exception(Exception exception);
        void Exception(string message, Exception exception);
    }
}