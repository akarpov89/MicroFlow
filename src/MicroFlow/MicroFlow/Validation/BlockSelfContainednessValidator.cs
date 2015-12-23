using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class BlockSelfContainednessValidator : VoidVisitor
    {
        private readonly BlockNode _block;
        private readonly IFlowNode _defaultCancellationHandler;
        private readonly IFlowNode _defaultErrorHandler;

        public BlockSelfContainednessValidator(
            [NotNull] BlockNode block,
            [CanBeNull] IFlowNode defaultErrorHandler, [CanBeNull] IFlowNode defaultCancellationHandler)
        {
            _block = block.NotNull();
            _defaultErrorHandler = defaultErrorHandler;
            _defaultCancellationHandler = defaultCancellationHandler;

            Result = new ValidationResult();
        }

        public ValidationResult Result { get; private set; }

        public ValidationResult Validate()
        {
            foreach (IFlowNode node in _block.InnerNodes)
            {
                node.Accept(this);
            }

            return Result;
        }

        protected override void VisitBlock(BlockNode blockNode)
        {
        }

        protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            VisitActivityNode(activityNode);
        }

        protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            CheckIfNodeIsInsideBlock(switchNode.DefaultCase);

            foreach (KeyValuePair<TChoice, IFlowNode> pair in switchNode.Cases)
            {
                CheckIfNodeIsInsideBlock(pair.Value);
            }
        }

        protected override void VisitCondition(DecisionNode decisionNode)
        {
            CheckIfNodeIsInsideBlock(decisionNode.WhenFalse);
            CheckIfNodeIsInsideBlock(decisionNode.WhenTrue);
        }

        protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            VisitActivityNode(forkJoinNode);
        }

        private void VisitActivityNode(ActivityNode activityNode)
        {
            CheckIfNodeIsInsideBlock(activityNode.PointsTo);
            CheckIfNodeIsInsideBlock(activityNode.FailureHandler);
            CheckIfNodeIsInsideBlock(activityNode.CancellationHandler);
        }

        private void CheckIfNodeIsInsideBlock(IFlowNode node)
        {
            if (node == null || node == _defaultErrorHandler || node == _defaultCancellationHandler) return;

            if (_block.InnerNodes.IndexOf(node) == -1)
            {
                Result.AddError(node, "Node is out of block " + _block);
            }
        }
    }
}