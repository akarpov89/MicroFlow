using MicroFlow;

namespace Sample
{
    internal class MyFlow : Flow
    {
        public override string Name => "My test flow";

        protected override void Build(FlowBuilder builder)
        {
            FaultHandlerNode<MyFaultHandlerActivity> faultHandler = builder.FaultHandler<MyFaultHandlerActivity>("Global error handler");
            ActivityNode<MyCancellationHandler> cancellationHandler =
                builder.Activity<MyCancellationHandler>("Global cancellation handler");

            builder.WithDefaultFaultHandler(faultHandler);
            builder.WithDefaultCancellationHandler(cancellationHandler);

            ActivityNode<InputActivity> inputFirst = builder.Activity<InputActivity>("Input first number");
            ActivityNode<InputActivity> inputSecond = builder.Activity<InputActivity>("Input second number");

            Result<int> first = Result<int>.Of(inputFirst);
            Result<int> second = Result<int>.Of(inputSecond);

            DecisionNode decision = builder.Decision("Check whether a first number is greater than a second");
            decision.WithCondition(() => first.CurrentValue > second.CurrentValue);

            ActivityNode<OutputActivity> outputWhenTrue =
                builder.Activity<OutputActivity>("Output condition when first greater than second");
            outputWhenTrue
                .Bind(x => x.Message)
                .To(() => $"{first.CurrentValue} > {second.CurrentValue}\r\n");

            ActivityNode<OutputActivity> outputWhenFalse =
                builder.Activity<OutputActivity>("Output condition when second greater than first");
            outputWhenFalse
                .Bind(x => x.Message)
                .To(() => $"{first.CurrentValue} <= {second.CurrentValue}\r\n");

            builder.Initial(inputFirst);

            inputFirst.ConnectTo(inputSecond);
            inputSecond.ConnectTo(decision);

            decision.ConnectTrueTo(outputWhenTrue)
                .ConnectFalseTo(outputWhenFalse);

            //var unused = builder.Activity<InputActivity>().WithName("Unused!");
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IReadService, ConsoleReadService>();
            services.AddSingleton<IWriteService, ConsoleWriteService>();
        }

        protected override ILogger CreateFlowExecutionLogger() => new ConsoleLogger {Verbosity = LogLevel.Error};
    }
}