using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
  [NotNull]
  public delegate TChoice ChoiceProvider<out TChoice>();

  public class SwitchNode<TChoice> : FlowNode
  {
    [NotNull] private readonly List<KeyValuePair<TChoice, IFlowNode>> myCases =
      new List<KeyValuePair<TChoice, IFlowNode>>();

    [CanBeNull] private ChoiceProvider<TChoice> myCompiledChoice;

    internal SwitchNode()
    {
    }

    public override FlowNodeKind Kind => FlowNodeKind.Switch;

    [CanBeNull]
    public Expression<ChoiceProvider<TChoice>> Choice { get; private set; }

    [NotNull]
    public IEnumerable<KeyValuePair<TChoice, IFlowNode>> Cases => myCases;

    [CanBeNull]
    public IFlowNode DefaultCase { get; private set; }

    public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
      return visitor.NotNull().VisitSwitch(this);
    }

    public override void RemoveConnections()
    {
      DefaultCase = null;
      myCases.Clear();

      Choice = null;
      myCompiledChoice = null;
    }

    [NotNull]
    public SwitchNode<TChoice> WithChoice([NotNull] Expression<ChoiceProvider<TChoice>> choiceExpression)
    {
      choiceExpression.AssertNotNull("choiceExpression != null");
      Choice.AssertIsNull("Choice is already set");

      Choice = choiceExpression;
      myCompiledChoice = Choice.Compile();
      return this;
    }

    [NotNull]
    public TChoice EvaluateChoice()
    {
      myCompiledChoice.AssertNotNull("Choice isn't set");
      return myCompiledChoice();
    }

    [NotNull]
    public IFlowNode Select([NotNull] TChoice choice)
    {
      IFlowNode node = FindCaseHandler(choice) ?? DefaultCase;
      node.AssertNotNull("No such case handler");

      return node;
    }

    public CaseBinder ConnectCase([NotNull] TChoice choice)
    {
      return new CaseBinder(this, choice);
    }

    [NotNull]
    public SwitchNode<TChoice> ConnectDefaultTo([NotNull] IFlowNode node)
    {
      node.AssertNotNull("node != null");
      DefaultCase.AssertIsNull("Default case is already set");

      DefaultCase = node;
      return this;
    }

    private void AddCase(TChoice choice, IFlowNode node)
    {
      myCases.Add(new KeyValuePair<TChoice, IFlowNode>(choice, node));
    }

    [CanBeNull]
    private IFlowNode FindCaseHandler(TChoice choice)
    {
      foreach (KeyValuePair<TChoice, IFlowNode> pair in myCases)
      {
        if (pair.Key.Equals(choice))
        {
          return pair.Value;
        }
      }

      return null;
    }

    public struct CaseBinder
    {
      private readonly SwitchNode<TChoice> mySwitchNode;
      private readonly TChoice myChoice;

      internal CaseBinder([NotNull] SwitchNode<TChoice> switchNode, [NotNull] TChoice choice)
      {
        mySwitchNode = switchNode;
        myChoice = choice;
      }

      [NotNull]
      public SwitchNode<TChoice> To([NotNull] IFlowNode node)
      {
        node.AssertNotNull("node != null");

        mySwitchNode.AddCase(myChoice, node);

        return mySwitchNode;
      }
    }
  }
}