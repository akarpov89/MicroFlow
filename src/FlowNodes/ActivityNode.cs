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

    public IFaultHandlerNode FaultHandler { get; internal set; }

    public override void RemoveConnections()
    {
      base.RemoveConnections();

      CancellationHandler = null;
      FaultHandler = null;
    }

    public abstract void RegisterActivityTaskHandler(ActivityTaskHandler handler);
  }

  public static class ActivityNodeExtensions
  {
    [NotNull]
    public static TActivityNode ConnectFaultTo<TActivityNode>(
      [NotNull] this TActivityNode from, [NotNull] IFaultHandlerNode to)
      where TActivityNode : ActivityNode
    {
      from.AssertNotNull("from != null");
      to.AssertNotNull("to != null");
      from.FaultHandler.AssertIsNull("Fault handler is already set");

      from.FaultHandler = to;
      to.SubscribeToExceptionsOf(from);
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