using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public interface IFlowNode
    {
        FlowNodeKind Kind { get; }

        Guid Id { get; }

        [CanBeNull]
        string Name { get; }

        [CanBeNull]
        TResult Accept<TResult>([NotNull] INodeVisitor<TResult> visitor);

        void RemoveConnections();
    }

    public interface IConnectableNode : IFlowNode
    {
        [CanBeNull]
        IFlowNode PointsTo { get; }
    }

    public interface IActivityNode : IConnectableNode
    {
        [CanBeNull]
        IFlowNode CancellationHandler { get; }

        [CanBeNull]
        IFaultHandlerNode FaultHandler { get; }

        void RegisterActivityTaskHandler([NotNull] ActivityTaskHandler handler);
    }

    public interface IFaultHandlerNode : IFlowNode
    {
        void SubscribeToExceptionsOf([NotNull] IActivityNode node);
    }

    public delegate void ActivityTaskHandler([NotNull] Task<object> activityTask);
}