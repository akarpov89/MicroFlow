namespace MicroFlow.Test
{
  public class Flow7 : Flow
  {
    public override string Name => "Flow7";

    protected override void Build(FlowBuilder builder)
    {
      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();

      builder.WithInitialNode(builder.Activity<ThrowServiceClient>("ThrowService client"));
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
      services.AddTransient<ThrowService, ThrowService>();
    }
  }
}