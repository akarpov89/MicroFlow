using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class FlowBuilder
    {
        [CanBeNull] private IFlowNode _initialNode;
        [CanBeNull] private IFaultHandlerNode _defaultFaultHandler;
        [CanBeNull] private IActivityNode _defaultCancellationHandler;

        [NotNull] private readonly Stack<BlockNode> _blockStack = new Stack<BlockNode>();

        [NotNull] private readonly List<IVariable> _globalVariables = new List<IVariable>();
        [NotNull] private readonly List<IFlowNode> _nodes = new List<IFlowNode>();

        private bool _isFreezed;

        [NotNull]
        public ActivityNode<TActivity> Activity<TActivity>()
            where TActivity : class, IActivity
        {
            return AddNode(new ActivityNode<TActivity>());
        }

        [NotNull]
        public ActivityNode<TActivity> Activity<TActivity>([NotNull] string name)
            where TActivity : class, IActivity
        {
            return Activity<TActivity>().WithName(name);
        }

        [NotNull]
        public SwitchNode<TChoice> SwitchOf<TChoice>()
        {
            return AddNode(new SwitchNode<TChoice>());
        }

        [NotNull]
        public SwitchNode<TChoice> SwitchOf<TChoice>([NotNull] string name)
        {
            return SwitchOf<TChoice>().WithName(name);
        }

        [NotNull]
        public ConditionNode Condition()
        {
            return AddNode(new ConditionNode());
        }

        [NotNull]
        public ConditionNode Condition([NotNull] string name)
        {
            return Condition().WithName(name);
        }

        [NotNull]
        public ForkJoinNode ForkJoin()
        {
            return AddNode(new ForkJoinNode());
        }

        [NotNull]
        public ForkJoinNode ForkJoin([NotNull] string name)
        {
            return ForkJoin().WithName(name);
        }

        [NotNull]
        public BlockNode Block()
        {
            return AddNode(new BlockNode());
        }

        [NotNull]
        public BlockNode Block([NotNull] string name)
        {
            return Block().WithName(name);
        }

        [NotNull]
        public BlockNode Block([NotNull] string name, [NotNull] BuildBlockAction buildBlockAction)
        {
            buildBlockAction.AssertNotNull("buildBlockAction != null");

            BlockNode block = Block(name);

            _blockStack.Push(block);
            buildBlockAction(block, this);
            _blockStack.Pop();

            return block;
        }

        [NotNull]
        public FaultHandlerNode<TActivity> FaultHandler<TActivity>()
            where TActivity : class, IFaultHandlerActivity
        {
            return AddNode(new FaultHandlerNode<TActivity>());
        }

        [NotNull]
        public FaultHandlerNode<TActivity> FaultHandler<TActivity>([NotNull] string name)
            where TActivity : class, IFaultHandlerActivity
        {
            return FaultHandler<TActivity>().WithName(name);
        }

        [NotNull]
        public Variable<T> Variable<T>(T initialValue = default(T))
        {
            var variable = new Variable<T>(initialValue);
            _globalVariables.Add(variable);
            return variable;
        }

        [NotNull]
        public FlowBuilder WithInitialNode([NotNull] IFlowNode node)
        {
            node.AssertNotNull("node != null");
            _initialNode.AssertIsNull("Initial node is already specified");
            node.AssertIsItemOf(_nodes, "Node must be part of the flow");
            _isFreezed.AssertFalse("Builder is freezed");

            _initialNode = node;
            return this;
        }

        [NotNull]
        public FlowBuilder WithDefaultFaultHandler([NotNull] IFaultHandlerNode handler)
        {
            handler.AssertNotNull("handler != null");
            _defaultFaultHandler.AssertIsNull("Default fault handler is already set");
            handler.AssertIsItemOf(_nodes, "Handler must be part of the flow");
            _isFreezed.AssertFalse("Builder is freezed");

            _defaultFaultHandler = handler;
            return this;
        }

        [NotNull]
        public FlowBuilder WithDefaultCancellationHandler<TCancellationHandler>(
            [NotNull] ActivityNode<TCancellationHandler> handler)
            where TCancellationHandler : class, IActivity
        {
            handler.AssertNotNull("handler != null");
            _defaultCancellationHandler.AssertIsNull("Default cancellation handler is already set");
            handler.AssertIsItemOf(_nodes, "Handler must be part of the flow");
            _isFreezed.AssertFalse("Builder is freezed");

            _defaultCancellationHandler = handler;
            return this;
        }

        [NotNull]
        public FlowDescription CreateFlow()
        {
            _isFreezed = true;

            return new FlowDescription(
                _initialNode,
                _defaultFaultHandler, _defaultCancellationHandler,
                new ReadOnlyCollection<IFlowNode>(_nodes), 
                new ReadOnlyCollection<IVariable>(_globalVariables));
        }

        public void Clear()
        {
            _defaultCancellationHandler = null;
            _defaultFaultHandler = null;

            foreach (IFlowNode node in _nodes)
            {
                node.RemoveConnections();
            }

            _nodes.Clear();

            foreach (IVariable variable in _globalVariables)
            {
                variable.RemoveBindings();
            }

            _globalVariables.Clear();
        }

        private TNode AddNode<TNode>(TNode node) where TNode : IFlowNode
        {
            _isFreezed.AssertFalse("Builder is freezed");

            _nodes.Add(node);

            if (_blockStack.Count > 0)
            {
                _blockStack.Peek().AddNode(node);
            }

            return node;
        }
    }
}