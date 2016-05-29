using System;
using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public class FlowPropertyInfo
  {
    public FlowPropertyInfo([NotNull] Type type, [NotNull] string name)
    {
      Type = type.NotNull();
      Name = name.NotNull();
    }

    public Type Type { get; }
    public string Name { get; }
  }
}