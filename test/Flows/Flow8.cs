namespace MicroFlow.Test
{
  public class Flow8 : Flow
  {
    public override string Name => "Flow8";

    protected override void Build(FlowBuilder builder)
    {
      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();

      var act1 = builder.Activity<DelayedIncrementActivity>("Delayed");
      act1.Bind(a => a.X).To(42);

      var act2 = builder.Activity<ThrowActivity>("Throw activity");

      act1.ConnectTo(act2);

      builder.WithInitialNode(act1);
    }
  }
}