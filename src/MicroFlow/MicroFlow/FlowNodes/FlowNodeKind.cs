using System;

namespace MicroFlow
{
    public enum FlowNodeKind
    {
        Activity,
        Decision,
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
                case FlowNodeKind.Decision:
                    return "Decision";
                case FlowNodeKind.Switch:
                    return "Switch";
                case FlowNodeKind.ForkJoin:
                    return "ForkJoin";
                case FlowNodeKind.Block:
                    return "Block";
                default:
                    throw new ArgumentOutOfRangeException("kind", kind, null);
            }
        }
    }
}