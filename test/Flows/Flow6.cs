namespace MicroFlow.Test
{
    public class Flow6 : Flow
    {
        public override string Name => "Flow6";
        protected override void Build(FlowBuilder builder)
        {
            builder.WithDefaultFaultHandler<MyFaultHandler>();
            builder.WithDefaultCancellationHandler<MyCancellationHandler>();

            var act1 = builder.DummyActivity();
            var act2 = builder.Activity<ThrowActivity>("Throw activity");

            act1.ConnectTo(act2);

            builder.WithInitialNode(act1);
        }
    }
}