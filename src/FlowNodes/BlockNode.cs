using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
  public class BlockNode : ConnectableNode
  {
    [NotNull] private readonly List<IVariable> myLocalVariables = new List<IVariable>();

    [NotNull] private readonly List<IFlowNode> myNodes = new List<IFlowNode>();

    internal BlockNode()
    {
    }

    public override FlowNodeKind Kind => FlowNodeKind.Block;

    [NotNull]
    public ReadOnlyCollection<IFlowNode> InnerNodes => new ReadOnlyCollection<IFlowNode>(myNodes);

    [NotNull]
    public ReadOnlyCollection<IVariable> LocalVariables => new ReadOnlyCollection<IVariable>(myLocalVariables);

    public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
      return visitor.NotNull().VisitBlock(this);
    }

    public override void RemoveConnections()
    {
      base.RemoveConnections();
      myNodes.Clear();
      myLocalVariables.Clear();
    }

    [NotNull]
    public BlockNode AddNode([NotNull] IFlowNode node)
    {
      node.AssertNotNull("node != null");
      node.AssertIsNotItemOf(myNodes, "Node is already in the block");

      myNodes.Add(node);
      return this;
    }

    [NotNull]
    public Variable<T> Variable<T>(T initialValue = default(T))
    {
      var variable = new Variable<T>(initialValue);
      myLocalVariables.Add(variable);
      return variable;
    }
  }
}