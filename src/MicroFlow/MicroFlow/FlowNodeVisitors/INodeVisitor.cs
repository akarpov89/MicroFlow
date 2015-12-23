using JetBrains.Annotations;

namespace MicroFlow
{
    public interface INodeVisitor<out TResult>
    {
        TResult VisitActivity<TActivity>([NotNull] ActivityNode<TActivity> activityNode)
            where TActivity : class, IActivity;

        TResult VisitSwitch<TChoice>([NotNull] SwitchNode<TChoice> switchNode);
        TResult VisitCondition([NotNull] DecisionNode decisionNode);
        TResult VisitForkJoin([NotNull] ForkJoinNode forkJoinNode);
        TResult VisitBlock([NotNull] BlockNode blockNode);
    }
}