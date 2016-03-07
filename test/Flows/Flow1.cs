namespace MicroFlow.Test
{
  public class Flow1 : Flow
  {
    public IReader Reader { get; set; }
    public IWriter Writer { get; set; }

    public override string Name => "Flow1. Uses condition node";

    protected override void Build(FlowBuilder builder)
    {
      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();

      var inputFirst = builder.Activity<ReadIntActivity>("Read first number");
      var inputSecond = builder.Activity<ReadIntActivity>("Read second number");

      var first = Result<int>.Of(inputFirst);
      var second = Result<int>.Of(inputSecond);

      var condition = builder.Condition("If first number > second number");
      condition.WithCondition(() => first.Get() > second.Get());

      var outputWhenTrue = builder.Activity<WriteMessageActivity>("Output: first > second");
      outputWhenTrue
        .Bind(x => x.Message)
        .To(() => $"{first.Get()} > {second.Get()}");

      var outputWhenFalse = builder.Activity<WriteMessageActivity>("Output: first <= second");

      outputWhenFalse
        .Bind(x => x.Message)
        .To(() => $"{first.Get()} <= {second.Get()}");

      builder.WithInitialNode(inputFirst);

      inputFirst.ConnectTo(inputSecond);
      inputSecond.ConnectTo(condition);

      condition
        .ConnectTrueTo(outputWhenTrue)
        .ConnectFalseTo(outputWhenFalse);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IReader>(Reader);
      services.AddSingleton<IWriter>(Writer);
    }
  }
}