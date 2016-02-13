using System.Collections.Generic;

namespace MicroFlow
{
  public sealed class ReachabilityValidator : FlowValidator
  {
    private readonly List<IFlowNode> myReachableNodes = new List<IFlowNode>();

    protected override void PerformGlobalValidation(FlowDescription flowDescription)
    {
      foreach (IFlowNode node in flowDescription.Nodes)
      {
        if (!myReachableNodes.Contains(node) &&
            node != flowDescription.InitialNode &&
            node != flowDescription.DefaultFaultHandler &&
            node != flowDescription.DefaultCancellationHandler)
        {
          Result.AddError(node, "Node isn't reachable from any other node");
        }
      }

      myReachableNodes.Clear();
    }

    protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
    {
      VisitActivityNode(activityNode);
    }

    protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
    {
      AddReachable(switchNode.DefaultCase);

      foreach (KeyValuePair<TChoice, IFlowNode> choiceToNode in switchNode.Cases)
      {
        AddReachable(choiceToNode.Value);
      }
    }

    protected override void VisitCondition(ConditionNode conditionNode)
    {
      AddReachable(conditionNode.WhenFalse);
      AddReachable(conditionNode.WhenTrue);
    }

    protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
    {
      VisitActivityNode(forkJoinNode);
    }

    protected override void VisitBlock(BlockNode blockNode)
    {
      if (blockNode.InnerNodes.Count > 0)
      {
        AddReachable(blockNode.InnerNodes[0]);
      }

      AddReachable(blockNode.PointsTo);
    }

    private void VisitActivityNode(IActivityNode node)
    {
      AddReachable(node.PointsTo);
      AddReachable(node.FaultHandler);
      AddReachable(node.CancellationHandler);
    }

    private void AddReachable(IFlowNode node)
    {
      if (node != null)
      {
        myReachableNodes.Add(node);
      }
    }
  }
}