using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class ConnectableNode : FlowNode, IConnectableNode
    {
        internal ConnectableNode()
        {
        }

        public IFlowNode PointsTo { get; internal set; }

        public override void RemoveConnections()
        {
            PointsTo = null;
        }
    }

    public static class ConnectableExtensions
    {
        [NotNull]
        public static TNode ConnectTo<TNode>([NotNull] this TNode from, [NotNull] IFlowNode to)
            where TNode : ConnectableNode
        {
            from.AssertNotNull("from != null");
            to.AssertNotNull("to != null");
            from.PointsTo.AssertIsNull("Connection is already set");

            from.PointsTo = to;
            return from;
        }
    }
}