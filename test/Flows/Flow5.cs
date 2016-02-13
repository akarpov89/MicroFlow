namespace MicroFlow.Test
{
  public class Flow5 : Flow
  {
    public override string Name => "Flow5";

    protected override void Build(FlowBuilder builder)
    {
      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();

      builder.WithInitialNode(builder.Activity<ThrowActivity>("Throw activity"));
    }
  }
}