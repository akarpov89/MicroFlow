using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class DefaultHandlersSetter : NodeVisitor
    {
        private readonly IActivityNode _cancellationHandler;
        private readonly IFaultHandlerNode _faultHandler;
        private readonly FlowBuilder _flowBuilder;

        public DefaultHandlersSetter([NotNull] FlowBuilder flowBuilder)
        {
            _flowBuilder = flowBuilder.NotNull();
            _faultHandler = flowBuilder.DefaultFaultHandler;
            _cancellationHandler = flowBuilder.DefaultCancellationHandler;
        }

        public void Execute()
        {
            if (_faultHandler != null || _cancellationHandler != null)
            {
                foreach (IFlowNode node in _flowBuilder.Nodes)
                {
                    node.Accept(this);
                }
            }
        }

        protected override void VisitCondition(ConditionNode conditionNode)
        {
        }

        protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
        {
            if (activityNode.FaultHandler == null &&
                activityNode != _faultHandler &&
                _faultHandler != null)
            {
                activityNode.ConnectFaultTo(_faultHandler);
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
            if (forkJoinNode.FaultHandler == null && _faultHandler != null)
            {
                forkJoinNode.ConnectFaultTo(_faultHandler);
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