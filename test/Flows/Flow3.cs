namespace MicroFlow.Test
{
    public class Flow3 : Flow
    {
        public IReader Reader { get; set; }
        public IWriter Writer { get; set; }

        public override string Name => "Flow3. Uses block and variable";

        protected override void Build(FlowBuilder builder)
        {
            builder.WithDefaultFaultHandler<MyFaultHandler>();
            builder.WithDefaultCancellationHandler<MyCancellationHandler>();

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
            services.AddSingleton<IReader>(Reader);
            services.AddSingleton<IWriter>(Writer);
        }
    }
}