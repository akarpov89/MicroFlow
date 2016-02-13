using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace MicroFlow
{
  public sealed class FlowDescription
  {
    internal FlowDescription(
      [CanBeNull] IFlowNode initialNode,
      [CanBeNull] IFaultHandlerNode defaultFaultHandler, [CanBeNull] IActivityNode defaultCancellationHandler,
      [NotNull] ReadOnlyCollection<IFlowNode> nodes,
      [NotNull] ReadOnlyCollection<IVariable> globalVariables)
    {
      InitialNode = initialNode;
      DefaultFaultHandler = defaultFaultHandler;
      DefaultCancellationHandler = defaultCancellationHandler;
      Nodes = nodes;
      GlobalVariables = globalVariables;
    }

    [CanBeNull]
    public IFlowNode InitialNode { get; }

    [CanBeNull]
    public IFaultHandlerNode DefaultFaultHandler { get; }

    [CanBeNull]
    public IActivityNode DefaultCancellationHandler { get; }

    [NotNull]
    public ReadOnlyCollection<IFlowNode> Nodes { get; }

    [NotNull]
    public ReadOnlyCollection<IVariable> GlobalVariables { get; }
  }
}