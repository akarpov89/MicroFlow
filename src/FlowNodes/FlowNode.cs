using System;
using JetBrains.Annotations;

namespace MicroFlow
{
  public abstract class FlowNode : IFlowNode
  {
    private string myName;

    public abstract FlowNodeKind Kind { get; }

    public Guid Id { get; } = Guid.NewGuid();

    public string Name
    {
      get { return myName; }
      set
      {
        value.AssertNotNullOrEmpty("Name cannot be null or empty");
        myName.AssertIsNull("Name is already set");

        myName = value;
      }
    }

    public abstract TResult Accept<TResult>(INodeVisitor<TResult> visitor);

    public abstract void RemoveConnections();

    public override string ToString()
    {
      return $"{{Node Kind: '{Kind}' Name: '{myName}'}}";
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