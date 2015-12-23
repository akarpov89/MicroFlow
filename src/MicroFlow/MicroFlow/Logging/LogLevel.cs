using System;

namespace MicroFlow
{
    [Flags]
    public enum LogLevel
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 4,
        All = Info | Warning | Error
    }
}