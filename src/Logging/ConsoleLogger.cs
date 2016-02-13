using System;

namespace MicroFlow
{
  public class ConsoleLogger : ILogger
  {
    public LogLevel Verbosity { get; set; }

    public void Write(LogLevel level, string message)
    {
      if ((Verbosity & level) == level)
      {
        Console.WriteLine(message);
      }
    }

    public void Write(LogLevel level, string format, object arg0)
    {
      if ((Verbosity & level) == level)
      {
        Console.WriteLine(format, arg0);
      }
    }

    public void Write(LogLevel level, string format, object arg0, object arg1)
    {
      if ((Verbosity & level) == level)
      {
        Console.WriteLine(format, arg0, arg1);
      }
    }

    public void Write(LogLevel level, string format, object arg0, object arg1, object arg2)
    {
      if ((Verbosity & level) == level)
      {
        Console.WriteLine(format, arg0, arg1, arg2);
      }
    }

    public void Write(LogLevel level, string format, params object[] args)
    {
      if ((Verbosity & level) == level)
      {
        Console.WriteLine(format, args);
      }
    }

    public void Exception(Exception exception)
    {
      Write(LogLevel.Error, exception.ToString());
    }

    public void Exception(string message, Exception exception)
    {
      Write(LogLevel.Error, "{0}\r\n{1}", message, exception);
    }
  }
}