using MicroFlow.Test;

namespace MicroFlow.Meta.Test
{
  public static class MetaFlow3
  {
    public static Flow Create()
    {
      var scheme = new FlowScheme("Flow3. Uses block and variable", "MicroFlow.Meta.Test.Flow3");

      scheme.DefaultFaultHandlerType = typeof(MyFaultHandler);
      scheme.DefaultCancellationHandlerType = typeof(MyCancellationHandler);

      scheme
        .AddProperty<IReader>("Reader")
        .AddProperty<IWriter>("Writer");

      var var = new VariableInfo(typeof(int), "var");

      var activity = new ActivityInfo(typeof(ReadIntActivity))
        .WithDescription("Input number")
        .AddVariableBinding(new VariableBindingInfo(var, VariableBindingKind.ActivityResult));

      var block = new BlockInfo()
        .WithDescription("MyBlock")
        .AddNode(activity);

      var outputActivity = new ActivityInfo(typeof(WriteMessageActivity))
        .WithDescription("Output activity")
        .AddPropertyBinding(new PropertyBindingInfo("Message", "() => $\"Echo: {var.CurrentValue}\""));
        
      scheme.AddNodes(block, outputActivity);
      scheme.AddVariable(var);

      scheme.IntialNode = block;
      block.ConnectTo(outputActivity);

      scheme
        .AddService(ServiceInfo.Singleton<IReader>("Reader"))
        .AddService(ServiceInfo.Singleton<IWriter>("Writer"));

      return scheme.EmitFlow();
    }
  }
}