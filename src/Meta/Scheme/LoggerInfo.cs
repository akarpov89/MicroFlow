using System;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class LoggerInfo
  {
    public LoggerInfo([NotNull] Type type)
    {
      Type = type.NotNull();
      Expression = null;
    }

    public LoggerInfo([NotNull] string expression)
    {
      Expression = expression.NotNull();
      Type = null;
    }

    [CanBeNull]
    public Type Type { get; }

    [CanBeNull]
    public string Expression { get; }
  }
}