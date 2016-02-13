using System.Collections.Generic;

namespace MicroFlow
{
  public sealed class NeighborsVisitor : INodeVisitor<IEnumerable<IFlowNode>>
  {
    public IEnumerable<IFlowNode> VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
      where TActivity : class, IActivity
    {
      if (activityNode.PointsTo != null) yield return activityNode.PointsTo;
      if (activityNode.FaultHandler != null) yield return activityNode.FaultHandler;
      if (activityNode.CancellationHandler != null) yield return activityNode.CancellationHandler;
    }

    public IEnumerable<IFlowNode> VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
    {
      if (switchNode.DefaultCase != null) yield return switchNode.DefaultCase;
      foreach (KeyValuePair<TChoice, IFlowNode> caseToNode in switchNode.Cases) yield return caseToNode.Value;
    }

    public IEnumerable<IFlowNode> VisitCondition(ConditionNode conditionNode)
    {
      if (conditionNode.WhenFalse != null) yield return conditionNode.WhenFalse;
      if (conditionNode.WhenTrue != null) yield return conditionNode.WhenTrue;
    }

    public IEnumerable<IFlowNode> VisitForkJoin(ForkJoinNode forkJoinNode)
    {
      if (forkJoinNode.PointsTo != null) yield return forkJoinNode.PointsTo;
      if (forkJoinNode.FaultHandler != null) yield return forkJoinNode.FaultHandler;
      if (forkJoinNode.CancellationHandler != null) yield return forkJoinNode.CancellationHandler;
    }

    public IEnumerable<IFlowNode> VisitBlock(BlockNode blockNode)
    {
      if (blockNode.PointsTo != null) yield return blockNode.PointsTo;
    }
  }

  public static class NeighborsExtension
  {
    private static readonly NeighborsVisitor Visitor = new NeighborsVisitor();

    public static IEnumerable<IFlowNode> GetNeighbors(this IFlowNode node)
    {
      return node.Accept(Visitor);
    }
  }
}