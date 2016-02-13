using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
  public struct ThenClause
  {
    [NotNull] private readonly FlowBuilder myBuilder;
    [NotNull] private readonly string myConditionDescription;
    [NotNull] private readonly Expression<Func<bool>> myCondition;

    internal ThenClause(
      [NotNull] FlowBuilder builder,
      [NotNull] string conditionDescription,
      [NotNull] Expression<Func<bool>> condition)
    {
      myBuilder = builder.NotNull();
      myConditionDescription = conditionDescription.NotNull();
      myCondition = condition.NotNull();

      ParentConditionNode = null;
      InitialConditionNode = null;
    }

    [CanBeNull]
    internal ConditionNode ParentConditionNode { get; set; }

    [CanBeNull]
    internal ConditionNode InitialConditionNode { get; set; }

    public ElseClause Then([NotNull] IFlowNode node)
    {
      var conditionNode = myBuilder
        .Condition(myConditionDescription)
        .WithCondition(myCondition)
        .ConnectTrueTo(node.NotNull());

      ParentConditionNode?.ConnectFalseTo(conditionNode);

      return new ElseClause(myBuilder, conditionNode)
      {
        InitialConditionNode = InitialConditionNode
      };
    }
  }

  public struct ElseClause
  {
    [NotNull] private readonly FlowBuilder myBuilder;
    [NotNull] private readonly ConditionNode myConditionNode;

    public ElseClause([NotNull] FlowBuilder builder, [NotNull] ConditionNode conditionNode)
    {
      myBuilder = builder.NotNull();
      myConditionNode = conditionNode.NotNull();

      InitialConditionNode = null;
    }

    internal ConditionNode InitialConditionNode { get; set; }

    public ConditionNode Else([NotNull] IFlowNode node)
    {
      myConditionNode.ConnectFalseTo(node.NotNull());

      return InitialConditionNode;
    }

    public ThenClause ElseIf([NotNull] string conditionDescription, [NotNull] Expression<Func<bool>> condition)
    {
      return new ThenClause(myBuilder, conditionDescription, condition)
      {
        ParentConditionNode = myConditionNode,
        InitialConditionNode = InitialConditionNode ?? myConditionNode
      };
    }
  }
}