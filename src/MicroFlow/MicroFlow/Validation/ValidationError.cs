using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class ValidationError
    {
        public ValidationError([NotNull] string message)
        {
            Message = message.NotNull();
        }

        public ValidationError([NotNull] IFlowNode node, [NotNull] string message)
        {
            node.AssertNotNull("node != null");

            NodeId = node.Id;
            NodeName = node.Name;
            Message = message.NotNull();
        }

        public ValidationError(Guid nodeId, [CanBeNull] string nodeName, [NotNull] string message)
        {
            NodeId = nodeId;
            NodeName = nodeName;
            Message = message.NotNull();
        }

        public Guid NodeId { get; private set; }

        [CanBeNull]
        public string NodeName { get; private set; }

        [NotNull]
        public string Message { get; private set; }

        public bool IsGlobal
        {
            get { return NodeId == Guid.Empty; }
        }
    }
}