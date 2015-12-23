using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class FlowNode : IFlowNode
    {
        private string _name;

        protected FlowNode()
        {
            Id = Guid.NewGuid();
        }

        public abstract FlowNodeKind Kind { get; }

        public Guid Id { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                value.AssertNotNullOrEmpty("Name cannot be null or empty");
                _name.AssertIsNull("Name is already set");

                _name = value;
            }
        }

        public abstract TResult Accept<TResult>(INodeVisitor<TResult> visitor);

        public abstract void RemoveConnections();

        public override string ToString()
        {
            return string.Format("{{Node Kind: '{0}' Name: '{1}'}}", Kind, _name);
        }
    }

    public static class FlowNodeExtensions
    {
        public static TFlowNode WithName<TFlowNode>([NotNull] this TFlowNode node, [NotNull] string name)
            where TFlowNode : FlowNode
        {
            node.NotNull().Name = name;
            return node;
        }
    }
}