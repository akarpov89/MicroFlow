namespace MicroFlow.Test
{
  public sealed class Flow4 : Flow
  {
    public override string Name => "Flow4";

    public int Number { get; set; }

    public IWriter Writer { get; set; }

    protected override void Build(FlowBuilder builder)
    {
      builder.WithDefaultFaultHandler<MyFaultHandler>();
      builder.WithDefaultCancellationHandler<MyCancellationHandler>();

      var a1 = builder.Activity<WriteMessageActivity>("1");
      a1.Bind(a => a.Message).To("1");

      var a2 = builder.Activity<WriteMessageActivity>("2");
      a2.Bind(a => a.Message).To("2");

      var a3 = builder.Activity<WriteMessageActivity>("3");
      a3.Bind(a => a.Message).To("3");

      var conditionNode = builder
        .If("If Number == 1", () => Number == 1).Then(a1)
        .ElseIf("If Number == 2", () => Number == 2).Then(a2)
        .Else(a3);

      builder.WithInitialNode(conditionNode);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
      services.AddSingleton<IWriter>(Writer);
    }
  }
}