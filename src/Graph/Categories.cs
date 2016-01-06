using System;

namespace MicroFlow.Graph
{
    public static class Categories
    {
        public const string ActivityNode = "Activity";
        public const string ConditionNode = "Condition";
        public const string SwitchNode = "Switch";
        public const string ForkJoinNode = "ForkJoin";
        public const string BlockNode = "Block";
        public const string ForkNode = "Fork";
        public const string FaultHandlerNode = "FaultHandler";

        public const string NormalFlowEdge = "Normal";
        public const string ParallelFlowEdge = "Parallel";
        public const string FaultFlowEdge = "Fault";
        public const string CancellationFlowEdge = "Cancellation";

        public static readonly string[] NodeCategories = 
        {
            ActivityNode, ConditionNode, SwitchNode, ForkJoinNode, ForkNode, BlockNode, FaultHandlerNode
        };
    }

    public static class CategoryHelper
    {
        public static string ToCategory(this IFlowNode node)
        {
            if (node is IFaultHandlerNode) return Categories.FaultHandlerNode;

            switch (node.Kind)
            {
                case FlowNodeKind.Activity:
                    return Categories.ActivityNode;
                case FlowNodeKind.Condition:
                    return Categories.ConditionNode;
                case FlowNodeKind.Switch:
                    return Categories.SwitchNode;
                case FlowNodeKind.ForkJoin:
                    return Categories.ForkJoinNode;
                case FlowNodeKind.Block:
                    return Categories.BlockNode;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}