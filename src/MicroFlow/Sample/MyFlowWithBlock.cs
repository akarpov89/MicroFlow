using MicroFlow;

namespace Sample
{
    internal class MyFlowWithBlock : Flow
    {
        public override string Name => "My third test flow";

        protected override void Build(FlowBuilder builder)
        {
            ErrorHandlerNode<MyErrorHandler> globalFailureHandler = builder.ErrorHandler<MyErrorHandler>();
            ActivityNode<MyCancellationHandler> globalCancellationHandler = builder.Activity<MyCancellationHandler>();

            builder.WithDefaultFailureHandler(globalFailureHandler);
            builder.WithDefaultCancellationHandler(globalCancellationHandler);

            Variable<int> myVar = builder.Variable<int>();

            BlockNode block = builder.Block("MyBlock", (thisBlock, blockBuilder) =>
            {
                ActivityNode<InputActivity> activity = blockBuilder.Activity<InputActivity>("Input number");
                myVar.BindToResultOf(activity);
            });

            ActivityNode<OutputActivity> outAct = builder.Activity<OutputActivity>("Output act");
            outAct.Bind(x => x.Message).To(() => $"You entered {myVar.CurrentValue}\r\n");

            builder.Initial(block);

            block.ConnectTo(outAct);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IReadService, ConsoleReadService>();
            services.AddSingleton<IWriteService, ConsoleWriteService>();
        }

        protected override ILogger CreateFlowExecutionLogger()
        {
            return new ConsoleLogger {Verbosity = LogLevel.All};
        }
    }
}