using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class BlockNode : ConnectableNode
    {
        [NotNull] private readonly List<IVariable> _localVariables = new List<IVariable>();

        [NotNull] private readonly List<IFlowNode> _nodes = new List<IFlowNode>();

        internal BlockNode()
        {
        }

        public override FlowNodeKind Kind
        {
            get { return FlowNodeKind.Block; }
        }

        [NotNull]
        public ReadOnlyCollection<IFlowNode> InnerNodes
        {
            get { return _nodes.AsReadOnly(); }
        }

        public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
        {
            return visitor.NotNull().VisitBlock(this);
        }

        public override void RemoveConnections()
        {
            base.RemoveConnections();
            _nodes.Clear();

            foreach (IVariable variable in _localVariables)
            {
                variable.RemoveBindings();
            }

            _localVariables.Clear();
        }

        [NotNull]
        public BlockNode AddNode([NotNull] IFlowNode node)
        {
            node.AssertNotNull("node != null");
            node.AssertIsNotItemOf(_nodes, "Node is already in the block");

            _nodes.Add(node);
            return this;
        }

        [NotNull]
        public Variable<T> Variable<T>(T initialValue = default(T))
        {
            var variable = new Variable<T>(initialValue);
            _localVariables.Add(variable);
            return variable;
        }
    }
}