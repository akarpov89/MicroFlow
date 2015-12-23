using System.Collections.Generic;

namespace MicroFlow
{
    public sealed class ReachabilityValidator : FlowValidator
    {
        private readonly List<IFlowNode> _reachableNodes = new List<IFlowNode>();

        protected override void PerformGlobalValidation(FlowBuilder flowBuilder)
        {
            foreach (IFlowNode node in flowBuilder.Nodes)
            {
                if (!_reachableNodes.Contains(node) &&
                    node != flowBuilder.InitialNode &&
                    node != flowBuilder.DefaultFailureHandler &&
                    node != flowBuilder.DefaultCancellationHandler)
                {
                    Result.AddError(node, "Node isn't reachable from any other node");
                }
            }

            _reachableNodes.Clear();
        }

        protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            VisitActivityNode(activityNode);
        }

        protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
            AddReachable(switchNode.DefaultCase);

            foreach (KeyValuePair<TChoice, IFlowNode> choiceToNode in switchNode.Cases)
            {
                AddReachable(choiceToNode.Value);
            }
        }

        protected override void VisitCondition(DecisionNode decisionNode)
        {
            AddReachable(decisionNode.WhenFalse);
            AddReachable(decisionNode.WhenTrue);
        }

        protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            VisitActivityNode(forkJoinNode);
        }

        protected override void VisitBlock(BlockNode blockNode)
        {
            if (blockNode.InnerNodes.Count > 0)
            {
                AddReachable(blockNode.InnerNodes[0]);
            }

            AddReachable(blockNode.PointsTo);
        }

        private void VisitActivityNode(IActivityNode node)
        {
            AddReachable(node.PointsTo);
            AddReachable(node.FailureHandler);
            AddReachable(node.CancellationHandler);
        }

        private void AddReachable(IFlowNode node)
        {
            if (node != null)
            {
                _reachableNodes.Add(node);
            }
        }
    }
}