using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class ConditionNode : FlowNode
    {
        [CanBeNull] private Func<bool> _compiledCondition;

        internal ConditionNode()
        {
        }

        public override FlowNodeKind Kind => FlowNodeKind.Condition;

        [CanBeNull]
        public Expression<Func<bool>> Condition { get; private set; }

        [CanBeNull]
        public IFlowNode WhenTrue { get; private set; }

        [CanBeNull]
        public IFlowNode WhenFalse { get; private set; }

        public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
        {
            return visitor.NotNull().VisitCondition(this);
        }

        public override void RemoveConnections()
        {
            WhenFalse = null;
            WhenTrue = null;

            Condition = null;
            _compiledCondition = null;
        }

        public bool EvaluateCondition()
        {
            _compiledCondition.AssertNotNull("Condition isn't set");
            return _compiledCondition();
        }

        [NotNull]
        public ConditionNode WithCondition([NotNull] Expression<Func<bool>> condition)
        {
            condition.AssertNotNull("condition != null");
            Condition.AssertIsNull("Condition is already set");

            Condition = condition;
            _compiledCondition = Condition.Compile();
            return this;
        }

        [NotNull]
        public ConditionNode ConnectTrueTo([NotNull] IFlowNode node)
        {
            node.AssertNotNull("node != null");
            WhenTrue.AssertIsNull("True branch is already set");

            WhenTrue = node;
            return this;
        }

        [NotNull]
        public ConditionNode ConnectFalseTo([NotNull] IFlowNode node)
        {
            node.AssertNotNull("node != null");
            WhenFalse.AssertIsNull("False branch is already set");

            WhenFalse = node;
            return this;
        }
    }
}