using JetBrains.Annotations;

namespace MicroFlow
{
  public sealed class DefaultHandlersSetter : NodeVisitor
  {
    private readonly IActivityNode myCancellationHandler;
    private readonly IFaultHandlerNode myFaultHandler;
    private readonly FlowDescription myFlowDescription;

    public DefaultHandlersSetter([NotNull] FlowDescription flowDescription)
    {
      myFlowDescription = flowDescription.NotNull();
      myFaultHandler = flowDescription.DefaultFaultHandler;
      myCancellationHandler = flowDescription.DefaultCancellationHandler;
    }

    public void Execute()
    {
      if (myFaultHandler != null || myCancellationHandler != null)
      {
        foreach (IFlowNode node in myFlowDescription.Nodes)
        {
          node.Accept(this);
        }
      }
    }

    protected override void VisitCondition(ConditionNode conditionNode)
    {
    }

    protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
    {
      if (activityNode.FaultHandler == null &&
          activityNode != myFaultHandler &&
          myFaultHandler != null)
      {
        activityNode.ConnectFaultTo(myFaultHandler);
      }

      if (activityNode.CancellationHandler == null &&
          activityNode != myCancellationHandler &&
          myCancellationHandler != null)
      {
        activityNode.ConnectCancellationTo(myCancellationHandler);
      }
    }

    protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
    {
    }

    protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
    {
      if (forkJoinNode.FaultHandler == null && myFaultHandler != null)
      {
        forkJoinNode.ConnectFaultTo(myFaultHandler);
      }

      if (forkJoinNode.CancellationHandler == null && myCancellationHandler != null)
      {
        forkJoinNode.ConnectCancellationTo(myCancellationHandler);
      }
    }

    protected override void VisitBlock(BlockNode blockNode)
    {
      if (blockNode.InnerNodes.Count == 0) return;

      foreach (IFlowNode blockItem in blockNode.InnerNodes)
      {
        blockItem.Accept(this);
      }
    }
  }
}