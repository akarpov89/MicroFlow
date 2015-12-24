using MicroFlow;

namespace Sample
{
    internal class MySecondFlow : Flow
    {
        public override string Name => "Test flow with fork";

        protected override void Build(FlowBuilder builder)
        {
            FaultHandlerNode<MyFaultHandlerActivity> eh = builder.FaultHandler<MyFaultHandlerActivity>();
            ActivityNode<MyCancellationHandler> ch = builder.Activity<MyCancellationHandler>();

            builder.WithDefaultFaultHandler(eh);
            builder.WithDefaultCancellationHandler(ch);

            ForkJoinNode forkJoin = builder.ForkJoin("My fork join node");

            ActivityDescriptor<MyAsyncActivity> first = forkJoin.Fork<MyAsyncActivity>("First fork");
            first.Bind(a => a.X).To(0);

            ActivityDescriptor<MyAsyncActivity> second = forkJoin.Fork<MyAsyncActivity>("Second fork");
            second.Bind(a => a.X).To(8);

            ActivityDescriptor<MyAsyncActivity> third = forkJoin.Fork<MyAsyncActivity>("Third fork");
            third.Bind(a => a.X).To(19);

            ActivityNode<MyAsyncSum> sum = builder.Activity<MyAsyncSum>("Sum action");
            sum.Bind(x => x.A).ToResultOf(first);
            sum.Bind(x => x.B).ToResultOf(second);
            sum.Bind(x => x.C).ToResultOf(third);

            builder.Initial(forkJoin);

            forkJoin.ConnectTo(sum);
        }

        protected override ILogger CreateFlowExecutionLogger()
        {
            return new ConsoleLogger {Verbosity = LogLevel.None};
        }
    }
}