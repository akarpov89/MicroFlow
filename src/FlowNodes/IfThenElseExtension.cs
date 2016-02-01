using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
    public struct ThenClause
    {
        [NotNull] private readonly FlowBuilder _builder;
        [NotNull] private readonly string _conditionDescription;
        [NotNull] private readonly Expression<Func<bool>> _condition;

        internal ThenClause(
            [NotNull] FlowBuilder builder,
            [NotNull] string conditionDescription,
            [NotNull] Expression<Func<bool>> condition)
        {
            _builder = builder.NotNull();
            _conditionDescription = conditionDescription.NotNull();
            _condition = condition.NotNull();

            ParentConditionNode = null;
            InitialConditionNode = null;
        }

        [CanBeNull]
        internal ConditionNode ParentConditionNode { get; set; }

        [CanBeNull]
        internal ConditionNode InitialConditionNode { get; set; }

        public ElseClause Then([NotNull] IFlowNode node)
        {
            var conditionNode = _builder
                .Condition(_conditionDescription)
                .WithCondition(_condition)
                .ConnectTrueTo(node.NotNull());

            ParentConditionNode?.ConnectFalseTo(conditionNode);

            return new ElseClause(_builder, conditionNode)
            {
                InitialConditionNode = InitialConditionNode
            };
        }
    }

    public struct ElseClause
    {
        [NotNull] private readonly FlowBuilder _builder;
        [NotNull] private readonly ConditionNode _conditionNode;

        public ElseClause([NotNull] FlowBuilder builder, [NotNull] ConditionNode conditionNode)
        {
            _builder = builder.NotNull();
            _conditionNode = conditionNode.NotNull();

            InitialConditionNode = null;
        }

        internal ConditionNode InitialConditionNode { get; set; }

        public ConditionNode Else([NotNull] IFlowNode node)
        {
            _conditionNode.ConnectFalseTo(node.NotNull());

            return InitialConditionNode;
        }

        public ThenClause ElseIf([NotNull] string conditionDescription, [NotNull] Expression<Func<bool>> condition)
        {
            return new ThenClause(_builder, conditionDescription, condition)
            {
                ParentConditionNode = _conditionNode,
                InitialConditionNode = InitialConditionNode ?? _conditionNode
            };
        }
    }
}