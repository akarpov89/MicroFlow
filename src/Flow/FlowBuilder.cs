using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
  public class FlowBuilder
  {
    [CanBeNull] private IFlowNode myInitialNode;
    [CanBeNull] private IFaultHandlerNode myDefaultFaultHandler;
    [CanBeNull] private IActivityNode myDefaultCancellationHandler;

    [NotNull] private readonly Stack<BlockNode> myBlockStack = new Stack<BlockNode>();

    [NotNull] private readonly List<IVariable> myGlobalVariables = new List<IVariable>();
    [NotNull] private readonly List<IFlowNode> myNodes = new List<IFlowNode>();

    private bool myIsFreezed;

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

      myBlockStack.Push(block);
      buildBlockAction(block, this);
      myBlockStack.Pop();

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
      myGlobalVariables.Add(variable);
      return variable;
    }

    [NotNull]
    public FlowBuilder WithInitialNode([NotNull] IFlowNode node)
    {
      node.AssertNotNull("node != null");
      myInitialNode.AssertIsNull("Initial node is already specified");
      node.AssertIsItemOf(myNodes, "Node must be part of the flow");
      myIsFreezed.AssertFalse("Builder is freezed");

      myInitialNode = node;
      return this;
    }

    [NotNull]
    public FlowBuilder WithDefaultFaultHandler([NotNull] IFaultHandlerNode handler)
    {
      handler.AssertNotNull("handler != null");
      myDefaultFaultHandler.AssertIsNull("Default fault handler is already set");
      handler.AssertIsItemOf(myNodes, "Handler must be part of the flow");
      myIsFreezed.AssertFalse("Builder is freezed");

      myDefaultFaultHandler = handler;
      return this;
    }

    [NotNull]
    public FlowBuilder WithDefaultCancellationHandler<TCancellationHandler>(
      [NotNull] ActivityNode<TCancellationHandler> handler)
      where TCancellationHandler : class, IActivity
    {
      handler.AssertNotNull("handler != null");
      myDefaultCancellationHandler.AssertIsNull("Default cancellation handler is already set");
      handler.AssertIsItemOf(myNodes, "Handler must be part of the flow");
      myIsFreezed.AssertFalse("Builder is freezed");

      myDefaultCancellationHandler = handler;
      return this;
    }

    [NotNull]
    public FlowDescription CreateFlow()
    {
      myIsFreezed = true;

      return new FlowDescription(
        myInitialNode,
        myDefaultFaultHandler, myDefaultCancellationHandler,
        new ReadOnlyCollection<IFlowNode>(myNodes),
        new ReadOnlyCollection<IVariable>(myGlobalVariables));
    }

    public void Clear()
    {
      myDefaultCancellationHandler = null;
      myDefaultFaultHandler = null;

      foreach (IFlowNode node in myNodes)
      {
        node.RemoveConnections();
      }

      myNodes.Clear();
      myGlobalVariables.Clear();
    }

    private TNode AddNode<TNode>(TNode node) where TNode : IFlowNode
    {
      myIsFreezed.AssertFalse("Builder is freezed");

      myNodes.Add(node);

      if (myBlockStack.Count > 0)
      {
        myBlockStack.Peek().AddNode(node);
      }

      return node;
    }
  }
}