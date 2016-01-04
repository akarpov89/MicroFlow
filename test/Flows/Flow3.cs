namespace MicroFlow.Test
{
    public class Flow3 : Flow
    {
        private readonly IReader _reader;
        private readonly IWriter _writer;

        public Flow3(IReader reader, IWriter writer)
        {
            _reader = reader;
            _writer = writer;
        }

        public override string Name => "Flow3. Uses block and variable";

        protected override void Build(FlowBuilder builder)
        {
            var faultHandler = builder.FaultHandler<MyFaultHandler>();
            var cancellationHandler = builder.Activity<MyCancellationHandler>();

            builder.WithDefaultFaultHandler(faultHandler);
            builder.WithDefaultCancellationHandler(cancellationHandler);

            var myVar = builder.Variable<int>();

            var block = builder.Block("MyBlock", (thisBlock, blockBuilder) =>
            {
                var activity = blockBuilder.Activity<ReadIntActivity>("Input number");
                myVar.BindToResultOf(activity);
            });

            var outputActivity = builder.Activity<WriteMessageActivity>("Output activity");
            outputActivity.Bind(x => x.Message).To(() => $"Echo: {myVar.CurrentValue}");

            builder.WithInitialNode(block);
            block.ConnectTo(outputActivity);
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IReader>(_reader);
            services.AddSingleton<IWriter>(_writer);
        }
    }
}