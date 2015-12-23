using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class DefaultHandlersSetter : UnitVisitor
    {
        private readonly IActivityNode _cancellationHandler;
        private readonly IErrorHandlerNode _errorHandler;
        private readonly FlowBuilder _flowBuilder;

        public DefaultHandlersSetter([NotNull] FlowBuilder flowBuilder)
        {
            _flowBuilder = flowBuilder.NotNull();
            _errorHandler = flowBuilder.DefaultFailureHandler;
            _cancellationHandler = flowBuilder.DefaultCancellationHandler;
        }

        public void Execute()
        {
            if (_errorHandler != null || _cancellationHandler != null)
            {
                foreach (IFlowNode node in _flowBuilder.Nodes)
                {
                    node.Accept(this);
                }
            }
        }

        protected override void VisitCondition(DecisionNode decisionNode)
        {
        }

        protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            if (activityNode.FailureHandler == null &&
                activityNode != _errorHandler &&
                _errorHandler != null)
            {
                activityNode.ConnectFailureTo(_errorHandler);
            }

            if (activityNode.CancellationHandler == null &&
                activityNode != _cancellationHandler &&
                _cancellationHandler != null)
            {
                activityNode.ConnectCancellationTo(_cancellationHandler);
            }
        }

        protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
        {
        }

        protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
        {
            if (forkJoinNode.FailureHandler == null && _errorHandler != null)
            {
                forkJoinNode.ConnectFailureTo(_errorHandler);
            }

            if (forkJoinNode.CancellationHandler == null && _cancellationHandler != null)
            {
                forkJoinNode.ConnectCancellationTo(_cancellationHandler);
            }
        }

        protected override void VisitBlock(BlockNode blockNode)
        {
            if (blockNode.InnerNodes.Count == 0) return;

            foreach (IFlowNode blockItem in blockNode.InnerNodes)
            {
                blockItem.Accept(this);
            }
        }
    }
}