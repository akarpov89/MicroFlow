using System;
using MicroFlow;

namespace Sample
{
    public class Class1
    {
        private void Foo()
        {
            var flow = new FlowBuilder();

            ErrorHandlerNode<MyErrorHandler> errorHandlerNode = flow.ErrorHandler<MyErrorHandler>();
            ActivityNode<MyCancellationHandler> cancelationHandlerNode = flow.Activity<MyCancellationHandler>();

            ActivityNode<SelectStrategy> selectStrategyNode = flow.Activity<SelectStrategy>()
                .ConnectFailureTo(errorHandlerNode)
                .ConnectCancellationTo(cancelationHandlerNode);

            SwitchNode<Strategy> strategySwitchNode = flow.SwitchOf<Strategy>();

            selectStrategyNode.ConnectTo(strategySwitchNode);

            strategySwitchNode
                .ConnectCase(Strategy.UpdateExisting).To(CreateXxx())
                .ConnectCase(Strategy.ReplaceAll).To(CreateYyy());

            var executor = new FlowRunner();

            executor.VisitActivity(selectStrategyNode);
        }

        private IFlowNode CreateYyy()
        {
            throw new NotImplementedException();
        }

        private IFlowNode CreateXxx()
        {
            throw new NotImplementedException();
        }
    }
}