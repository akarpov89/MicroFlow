using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class FlowBuilder
    {
        [NotNull] private readonly Stack<BlockNode> _blockStack = new Stack<BlockNode>();

        [NotNull] private readonly List<IVariable> _globalVariables = new List<IVariable>();

        [NotNull] private readonly List<IFlowNode> _nodes = new List<IFlowNode>();

        [CanBeNull]
        public IErrorHandlerNode DefaultFailureHandler { get; private set; }

        [CanBeNull]
        public IActivityNode DefaultCancellationHandler { get; private set; }

        [CanBeNull]
        public IFlowNode InitialNode { get; private set; }

        [NotNull]
        public ReadOnlyCollection<IFlowNode> Nodes
        {
            get { return _nodes.AsReadOnly(); }
        }

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
        public DecisionNode Decision()
        {
            return AddNode(new DecisionNode());
        }

        [NotNull]
        public DecisionNode Decision([NotNull] string name)
        {
            return Decision().WithName(name);
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
        public ErrorHandlerNode<TActivity> ErrorHandler<TActivity>()
            where TActivity : class, IErrorHandler
        {
            return AddNode(new ErrorHandlerNode<TActivity>());
        }

        [NotNull]
        public ErrorHandlerNode<TActivity> ErrorHandler<TActivity>([NotNull] string name)
            where TActivity : class, IErrorHandler
        {
            return ErrorHandler<TActivity>().WithName(name);
        }

        [NotNull]
        public Variable<T> Variable<T>(T initialValue = default(T))
        {
            var variable = new Variable<T>(initialValue);
            _globalVariables.Add(variable);
            return variable;
        }

        [NotNull]
        public FlowBuilder Initial([NotNull] IFlowNode node)
        {
            node.AssertNotNull("node != null");
            InitialNode.AssertIsNull("Initial node is already specified");
            node.AssertIsItemOf(_nodes, "Node must be part of the flow");

            InitialNode = node;
            return this;
        }

        [NotNull]
        public FlowBuilder WithDefaultFailureHandler([NotNull] IErrorHandlerNode handler)
        {
            handler.AssertNotNull("handler != null");
            DefaultFailureHandler.AssertIsNull("Default failure handler is already set");
            handler.AssertIsItemOf(_nodes, "Handler must be part of the flow");

            DefaultFailureHandler = handler;
            return this;
        }

        [NotNull]
        public FlowBuilder WithDefaultCancellationHandler<TCancellationHandler>(
            [NotNull] ActivityNode<TCancellationHandler> handler)
            where TCancellationHandler : class, IActivity
        {
            handler.AssertNotNull("handler != null");
            DefaultCancellationHandler.AssertIsNull("Default cancellation handler is already set");
            handler.AssertIsItemOf(_nodes, "Handler must be part of the flow");

            DefaultCancellationHandler = handler;
            return this;
        }

        public void Clear()
        {
            DefaultCancellationHandler = null;
            DefaultFailureHandler = null;

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
            _nodes.Add(node);

            if (_blockStack.Count > 0)
            {
                _blockStack.Peek().AddNode(node);
            }

            return node;
        }
    }
}