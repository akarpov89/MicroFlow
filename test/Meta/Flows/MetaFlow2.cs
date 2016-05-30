using MicroFlow.Test;

namespace MicroFlow.Meta.Test
{
  public static class MetaFlow2
  {
    public static Flow Create()
    {
      var scheme = new FlowScheme("Flow2. Uses fork-join node", "MicroFlow.Meta.Test.Flow2");

      scheme.DefaultFaultHandlerType = typeof(MyFaultHandler);
      scheme.DefaultCancellationHandlerType = typeof(MyCancellationHandler);

      scheme
        .AddProperty<IWriter>("Writer")
        .AddProperty<int>("A")
        .AddProperty<int>("B")
        .AddProperty<int>("C");

      var first = new ActivityInfo(typeof(DelayedIncrementActivity))
        .WithDescription("First work")
        .AddPropertyBinding(new PropertyBindingInfo("X", "A"));

      var second = new ActivityInfo(typeof(DelayedIncrementActivity))
        .WithDescription("Second work")
        .AddPropertyBinding(new PropertyBindingInfo("X", "B"));

      var third = new ActivityInfo(typeof(DelayedIncrementActivity))
        .WithDescription("First work")
        .AddPropertyBinding(new PropertyBindingInfo("X", "C"));

      var forkJoin = new ForkJoinInfo()
        .WithDescription("My fork join node")
        .AddForks(first, second, third);

      var sum = new ActivityInfo(typeof(SumActivity))
        .WithDescription("Sum action")
        .AddPropertyBinding(new PropertyBindingInfo("A", PropertyBindingKind.ActivityResult) {Activity = first})
        .AddPropertyBinding(new PropertyBindingInfo("B", PropertyBindingKind.ActivityResult) {Activity = second})
        .AddPropertyBinding(new PropertyBindingInfo("C", PropertyBindingKind.ActivityResult) {Activity = third});

      scheme.AddNodes(forkJoin, sum);
      scheme.IntialNode = forkJoin;
      forkJoin.ConnectTo(sum);

      scheme.AddService(ServiceInfo.Singleton<IWriter>("Writer"));

      return scheme.EmitFlow();
    }
  }
}