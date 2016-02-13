using System;

namespace MicroFlow
{
  public enum FlowNodeKind
  {
    Activity,
    Condition,
    Switch,
    ForkJoin,
    Block
  }

  public static class FlowNodeKindExtensions
  {
    public static string ToText(this FlowNodeKind kind)
    {
      switch (kind)
      {
        case FlowNodeKind.Activity:
          return "Activity";
        case FlowNodeKind.Condition:
          return "Condition";
        case FlowNodeKind.Switch:
          return "Switch";
        case FlowNodeKind.ForkJoin:
          return "ForkJoin";
        case FlowNodeKind.Block:
          return "Block";
        default:
          throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
      }
    }
  }
}