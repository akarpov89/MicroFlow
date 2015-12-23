using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class UnitVisitor : INodeVisitor<Unit>
    {
        Unit INodeVisitor<Unit>.VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            VisitActivity(activityNode);
            return Unit.Instance;
        }

        Unit INodeVisitor<Unit>.VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            VisitSwitch(switchNode);
            return Unit.Instance;
        }

        Unit INodeVisitor<Unit>.VisitCondition(DecisionNode decisionNode)
        {
            VisitCondition(decisionNode);
            return Unit.Instance;
        }

        Unit INodeVisitor<Unit>.VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            VisitForkJoin(forkJoinNode);
            return Unit.Instance;
        }

        Unit INodeVisitor<Unit>.VisitBlock(BlockNode blockNode)
        {
            VisitBlock(blockNode);
            return Unit.Instance;
        }

        protected abstract void VisitActivity<TActivity>([NotNull] ActivityNode<TActivity> activityNode)
            where TActivity : class, IActivity;

        protected abstract void VisitSwitch<TChoice>([NotNull] SwitchNode<TChoice> switchNode);
        protected abstract void VisitCondition([NotNull] DecisionNode decisionNode);
        protected abstract void VisitForkJoin([NotNull] ForkJoinNode forkJoinNode);
        protected abstract void VisitBlock([NotNull] BlockNode blockNode);
    }
}