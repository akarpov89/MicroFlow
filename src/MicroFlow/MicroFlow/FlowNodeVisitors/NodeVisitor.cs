using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class NodeVisitor : INodeVisitor<Null>
    {
        Null INodeVisitor<Null>.VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            VisitActivity(activityNode);
            return null;
        }

        Null INodeVisitor<Null>.VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            VisitSwitch(switchNode);
            return null;
        }

        Null INodeVisitor<Null>.VisitCondition(ConditionNode conditionNode)
        {
            VisitCondition(conditionNode);
            return null;
        }

        Null INodeVisitor<Null>.VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            VisitForkJoin(forkJoinNode);
            return null;
        }

        Null INodeVisitor<Null>.VisitBlock(BlockNode blockNode)
        {
            VisitBlock(blockNode);
            return null;
        }

        protected abstract void VisitActivity<TActivity>([NotNull] ActivityNode<TActivity> activityNode)
            where TActivity : class, IActivity;

        protected abstract void VisitSwitch<TChoice>([NotNull] SwitchNode<TChoice> switchNode);
        protected abstract void VisitCondition([NotNull] ConditionNode conditionNode);
        protected abstract void VisitForkJoin([NotNull] ForkJoinNode forkJoinNode);
        protected abstract void VisitBlock([NotNull] BlockNode blockNode);
    }
}