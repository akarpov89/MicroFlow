using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MicroFlow.Meta
{
  public enum NodeKind
  {
    Activity,
    Condition,
    Switch,
    ForkJoin,
    Block
  }

  public abstract class NodeInfo
  {
    public abstract NodeKind Kind { get; }

    [CanBeNull]
    public string Description { get; set; }

    public abstract void Accept([NotNull] INodeVisitor visitor);
  }

  public abstract class ActivityLikeNodeInfo : NodeInfo
  {
    [CanBeNull]
    public NodeInfo PointsTo { get; set; }

    [CanBeNull]
    public NodeInfo FaultHandler { get; set; }

    [CanBeNull]
    public NodeInfo CancellationHandler { get; set; }
  }

  public sealed class ActivityInfo : ActivityLikeNodeInfo
  {
    public ActivityInfo([NotNull] Type activityType)
    {
      ActivityType = activityType.NotNull();
      PropertyBindings = new List<PropertyBindingInfo>();
      VariableBindings = new List<VariableBindingInfo>();
    }

    public override NodeKind Kind => NodeKind.Activity;

    public bool IsFaultHandler { get; set; }

    [NotNull]
    public Type ActivityType { get; }

    [NotNull]
    public List<PropertyBindingInfo> PropertyBindings { get; }

    [NotNull]
    public List<VariableBindingInfo> VariableBindings { get; }

    [CanBeNull]
    public VariableInfo Result { get; set; }

    public override void Accept(INodeVisitor visitor)
    {
      visitor.Visit(this);
    }

    [NotNull]
    public ActivityInfo AddPropertyBinding([NotNull] PropertyBindingInfo binding)
    {
      PropertyBindings.Add(binding.NotNull());
      return this;
    }

    [NotNull]
    public ActivityInfo AddVariableBinding([NotNull] VariableBindingInfo binding)
    {
      VariableBindings.Add(binding.NotNull());
      return this;
    }

    [NotNull]
    public ActivityInfo WithResult([NotNull] Type resultType, [NotNull] string variableName)
    {
      Result = new VariableInfo(resultType, variableName);
      return this;
    }

    [NotNull]
    public ActivityInfo WithResult<TResult>([NotNull] string variableName)
    {
      return WithResult(typeof(TResult), variableName);
    }
  }

  public sealed class ConditionInfo : NodeInfo
  {
    public ConditionInfo([NotNull] string conditionExpression)
    {
      Expression = conditionExpression.NotNull();
    }

    public override NodeKind Kind => NodeKind.Condition;

    [NotNull]
    public string Expression { get; }

    [CanBeNull]
    public NodeInfo WhenTrue { get; set; }

    [CanBeNull]
    public NodeInfo WhenFalse { get; set; }

    public override void Accept(INodeVisitor visitor)
    {
      visitor.Visit(this);
    }
  }

  public sealed class SwitchInfo : NodeInfo
  {
    public SwitchInfo([NotNull] Type type, [NotNull] string expression)
    {
      Type = type.NotNull();
      Expression = expression.NotNull();
      Cases = new List<CaseInfo>();
    }

    public override NodeKind Kind => NodeKind.Switch;

    [NotNull]
    public Type Type { get; }

    [NotNull]
    public string Expression { get; }

    [NotNull]
    public List<CaseInfo> Cases { get; }

    [CanBeNull]
    public NodeInfo DefaultCase { get; set; }

    public override void Accept(INodeVisitor visitor)
    {
      visitor.Visit(this);
    }

    [NotNull]
    public SwitchInfo AddCase([NotNull] CaseInfo caseInfo)
    {
      Cases.Add(caseInfo.NotNull());
      return this;
    }

    [NotNull]
    public SwitchInfo AddCase([NotNull] string valueExpression, [NotNull] NodeInfo handler)
    {
      return AddCase(new CaseInfo(valueExpression, handler));
    }
  }

  public sealed class CaseInfo
  {
    public CaseInfo([NotNull] string valueExpression, [NotNull] NodeInfo handler)
    {
      ValueExpression = valueExpression.NotNull();
      Handler = handler;
    }

    [NotNull]
    public string ValueExpression { get; }

    [NotNull]
    public NodeInfo Handler { get; }
  }

  public class ForkJoinInfo : ActivityLikeNodeInfo
  {
    public ForkJoinInfo()
    {
      Forks = new List<ActivityInfo>();
    }

    public override NodeKind Kind => NodeKind.ForkJoin;

    [NotNull]
    public List<ActivityInfo> Forks { get; }

    public override void Accept(INodeVisitor visitor)
    {
      visitor.Visit(this);
    }

    [NotNull]
    public ForkJoinInfo AddFork([NotNull] ActivityInfo fork)
    {
      Forks.Add(fork.NotNull());
      return this;
    }

    [NotNull]
    public ForkJoinInfo AddForks([NotNull] params ActivityInfo[] forks)
    {
      forks.AssertNotNull("forks != null");

      foreach (var fork in forks)
      {
        AddFork(fork);
      }

      return this;
    }
  }

  public class BlockInfo : NodeInfo
  {
    public BlockInfo()
    {
      Nodes = new List<NodeInfo>();
      Variables = new List<VariableInfo>();
    }

    public override NodeKind Kind => NodeKind.Block;

    [CanBeNull]
    public NodeInfo PointsTo { get; set; }

    [NotNull]
    public List<NodeInfo> Nodes { get; }

    [NotNull]
    public List<VariableInfo> Variables { get; }

    public override void Accept(INodeVisitor visitor)
    {
      visitor.Visit(this);
    }

    [NotNull]
    public BlockInfo AddNode([NotNull] NodeInfo node)
    {
      Nodes.Add(node.NotNull());
      return this;
    }

    [NotNull]
    public BlockInfo AddNodes([NotNull] params NodeInfo[] nodes)
    {
      nodes.AssertNotNull("nodes != null");

      foreach (var node in nodes)
      {
        AddNode(node);
      }

      return this;
    }

    [NotNull]
    public BlockInfo AddVariable([NotNull] VariableInfo variable)
    {
      Variables.Add(variable.NotNull());
      return this;
    }

    [NotNull]
    public BlockInfo AddVariables([NotNull] params VariableInfo[] variables)
    {
      variables.AssertNotNull("variables != null");

      foreach (var variable in variables)
      {
        AddVariable(variable);
      }

      return this;
    }
  }

  public enum PropertyBindingKind
  {
    ActivityResult,
    ActivityException,
    Expression
  }

  public class PropertyBindingInfo
  {
    public PropertyBindingInfo([NotNull] string property, PropertyBindingKind bindingKind)
    {
      Property = property.NotNull();
      Kind = bindingKind;
    }

    public PropertyBindingInfo([NotNull] string property, [NotNull] string expression)
    {
      Property = property.NotNull();
      Kind = PropertyBindingKind.Expression;
      BindingExpression = expression.NotNull();
    }

    public PropertyBindingKind Kind { get; }

    [NotNull]
    public string Property { get; }

    [CanBeNull]
    public ActivityInfo Activity { get; set; }

    [CanBeNull]
    public string BindingExpression { get; set; }
  }
}