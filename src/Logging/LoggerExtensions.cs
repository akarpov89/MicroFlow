using JetBrains.Annotations;

namespace MicroFlow
{
  public static class LoggerExtensions
  {
    public static void Info(this ILogger log, string message)
    {
      log.Write(LogLevel.Info, message);
    }

    [StringFormatMethod("format")]
    public static void Info(this ILogger log, string format, object arg0)
    {
      log.Write(LogLevel.Info, format, arg0);
    }

    [StringFormatMethod("format")]
    public static void Info(this ILogger log, string format, object arg0, object arg1)
    {
      log.Write(LogLevel.Info, format, arg0, arg1);
    }

    [StringFormatMethod("format")]
    public static void Info(this ILogger log, string format, object arg0, object arg1, object arg2)
    {
      log.Write(LogLevel.Info, format, arg0, arg1, arg2);
    }

    [StringFormatMethod("format")]
    public static void Info(this ILogger log, string format, params object[] args)
    {
      log.Write(LogLevel.Info, format, args);
    }

    public static void Warning(this ILogger log, string message)
    {
      log.Write(LogLevel.Warning, message);
    }

    [StringFormatMethod("format")]
    public static void Warning(this ILogger log, string format, object arg0)
    {
      log.Write(LogLevel.Warning, format, arg0);
    }

    [StringFormatMethod("format")]
    public static void Warning(this ILogger log, string format, object arg0, object arg1)
    {
      log.Write(LogLevel.Warning, format, arg0, arg1);
    }

    [StringFormatMethod("format")]
    public static void Warning(this ILogger log, string format, object arg0, object arg1, object arg2)
    {
      log.Write(LogLevel.Warning, format, arg0, arg1, arg2);
    }

    [StringFormatMethod("format")]
    public static void Warning(this ILogger log, string format, params object[] args)
    {
      log.Write(LogLevel.Warning, format, args);
    }

    public static void Error(this ILogger log, string message)
    {
      log.Write(LogLevel.Error, message);
    }

    [StringFormatMethod("format")]
    public static void Error(this ILogger log, string format, object arg0)
    {
      log.Write(LogLevel.Error, format, arg0);
    }

    [StringFormatMethod("format")]
    public static void Error(this ILogger log, string format, object arg0, object arg1)
    {
      log.Write(LogLevel.Error, format, arg0, arg1);
    }

    [StringFormatMethod("format")]
    public static void Error(this ILogger log, string format, object arg0, object arg1, object arg2)
    {
      log.Write(LogLevel.Error, format, arg0, arg1, arg2);
    }

    [StringFormatMethod("format")]
    public static void Error(this ILogger log, string format, params object[] args)
    {
      log.Write(LogLevel.Error, format, args);
    }
  }
}