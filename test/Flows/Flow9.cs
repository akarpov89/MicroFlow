namespace MicroFlow.Test
{
  public class Flow9 : Flow<int>
  {
    public IReader Reader { get; set; }

    public override string Name => "Flow9";

    protected override void Build(FlowBuilder builder)
    {
      var inputFirst = builder.Activity<ReadIntActivity>("Read first number");
      var inputSecond = builder.Activity<ReadIntActivity>("Read second number");

      var first = Result<int>.Of(inputFirst);
      var second = Result<int>.Of(inputSecond);

      inputSecond.OnCompletionAssign(Result, () => first.Get() + second.Get());

      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();
      builder.WithInitialNode(inputFirst);

      inputFirst.ConnectTo(inputSecond);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IReader>(Reader);
    }
  }
}