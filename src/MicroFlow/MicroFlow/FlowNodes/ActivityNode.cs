using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class ActivityNode : ConnectableNode, IActivityNode
    {
        internal ActivityNode()
        {
        }

        public IFlowNode CancellationHandler { get; internal set; }

        public IErrorHandlerNode FailureHandler { get; internal set; }

        public override void RemoveConnections()
        {
            base.RemoveConnections();

            CancellationHandler = null;
            FailureHandler = null;
        }

        public abstract void RegisterActivityTaskHandler(ActivityTaskHandler handler);
    }

    public static class ActivityNodeExtensions
    {
        [NotNull]
        public static TActivityNode ConnectFailureTo<TActivityNode>(
            [NotNull] this TActivityNode from, [NotNull] IErrorHandlerNode to)
            where TActivityNode : ActivityNode
        {
            from.AssertNotNull("from != null");
            to.AssertNotNull("to != null");
            from.FailureHandler.AssertIsNull("Failure handler is already set");

            from.FailureHandler = to;
            to.SubscribeToErrorsOf(from);
            return from;
        }

        [NotNull]
        public static TActivityNode ConnectCancellationTo<TActivityNode>(
            [NotNull] this TActivityNode from, [NotNull] IActivityNode to)
            where TActivityNode : ActivityNode
        {
            from.AssertNotNull("from != null");
            to.AssertNotNull("to != null");
            from.CancellationHandler.AssertIsNull("Cancellation handler is already set");

            from.CancellationHandler = to;
            return from;
        }
    }
}