using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
    [NotNull]
    public delegate TChoice ChoiceProvider<out TChoice>();

    public class SwitchNode<TChoice> : FlowNode
    {
        [NotNull] private readonly List<KeyValuePair<TChoice, IFlowNode>> _cases =
            new List<KeyValuePair<TChoice, IFlowNode>>();

        [CanBeNull] private ChoiceProvider<TChoice> _compiledChoice;

        internal SwitchNode()
        {
        }

        public override FlowNodeKind Kind
        {
            get { return FlowNodeKind.Switch; }
        }

        [CanBeNull]
        public Expression<ChoiceProvider<TChoice>> Choice { get; private set; }

        [NotNull]
        public IEnumerable<KeyValuePair<TChoice, IFlowNode>> Cases
        {
            get { return _cases; }
        }

        [CanBeNull]
        public IFlowNode DefaultCase { get; private set; }

        public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
        {
            return visitor.NotNull().VisitSwitch(this);
        }

        public override void RemoveConnections()
        {
            DefaultCase = null;
            _cases.Clear();

            Choice = null;
            _compiledChoice = null;
        }

        [NotNull]
        public SwitchNode<TChoice> WithChoice([NotNull] Expression<ChoiceProvider<TChoice>> choiceFunc)
        {
            choiceFunc.AssertNotNull("choiceFunc != null");
            Choice.AssertIsNull("Choice is already set");

            Choice = choiceFunc;
            _compiledChoice = Choice.Compile();
            return this;
        }

        [NotNull]
        public TChoice EvaluateChoice()
        {
            _compiledChoice.AssertNotNull("Choice isn't set");
            return _compiledChoice();
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
            _cases.Add(new KeyValuePair<TChoice, IFlowNode>(choice, node));
        }

        [CanBeNull]
        private IFlowNode FindCaseHandler(TChoice choice)
        {
            foreach (KeyValuePair<TChoice, IFlowNode> pair in _cases)
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
            private readonly SwitchNode<TChoice> _switchNode;
            private readonly TChoice _choice;

            internal CaseBinder([NotNull] SwitchNode<TChoice> switchNode, [NotNull] TChoice choice)
            {
                _switchNode = switchNode;
                _choice = choice;
            }

            [NotNull]
            public SwitchNode<TChoice> To([NotNull] IFlowNode node)
            {
                node.AssertNotNull("node != null");

                _switchNode.AddCase(_choice, node);

                return _switchNode;
            }
        }
    }
}