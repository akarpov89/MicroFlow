using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class VoidVisitor : INodeVisitor<Void>
    {
        Void INodeVisitor<Void>.VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            VisitActivity(activityNode);
            return Void.Instance;
        }

        Void INodeVisitor<Void>.VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            VisitSwitch(switchNode);
            return Void.Instance;
        }

        Void INodeVisitor<Void>.VisitCondition(DecisionNode decisionNode)
        {
            VisitCondition(decisionNode);
            return Void.Instance;
        }

        Void INodeVisitor<Void>.VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            VisitForkJoin(forkJoinNode);
            return Void.Instance;
        }

        Void INodeVisitor<Void>.VisitBlock(BlockNode blockNode)
        {
            VisitBlock(blockNode);
            return Void.Instance;
        }

        protected abstract void VisitActivity<TActivity>([NotNull] ActivityNode<TActivity> activityNode)
            where TActivity : class, IActivity;

        protected abstract void VisitSwitch<TChoice>([NotNull] SwitchNode<TChoice> switchNode);
        protected abstract void VisitCondition([NotNull] DecisionNode decisionNode);
        protected abstract void VisitForkJoin([NotNull] ForkJoinNode forkJoinNode);
        protected abstract void VisitBlock([NotNull] BlockNode blockNode);
    }
}