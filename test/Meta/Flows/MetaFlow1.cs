using System;
using MicroFlow.Test;

namespace MicroFlow.Meta.Test
{
  public static class MetaFlow1
  {
    public static Flow Create()
    {
      var scheme = new FlowScheme("Flow1. Uses condition node", "MicroFlow.Meta.Test.Flow1");

      scheme.DefaultFaultHandlerType = typeof(MyFaultHandler);
      scheme.DefaultCancellationHandlerType = typeof(MyCancellationHandler);

      var inputFirst = new ActivityInfo(typeof(ReadIntActivity))
      {
        Description = "Read first number",
        Result = new VariableInfo(typeof(int), "first")
      };

      var inputSecond = new ActivityInfo(typeof(ReadIntActivity))
      {
        Description = "Read second number",
        Result = new VariableInfo(typeof(int), "second")
      };

      var condition = new ConditionInfo("() => first.Get() > second.Get()")
      {
        Description = "If first number > second number"
      };

      var outputWhenTrue = new ActivityInfo(typeof(WriteMessageActivity))
      {
        Description = "Output: first > second"
      };

      outputWhenTrue.AddBinding(new PropertyBindingInfo("Message", PropertyBindingKind.Expression)
      {
        BindingExpression = "() => $\"{first.Get()} > {second.Get()}\""
      });

      var outputWhenFalse = new ActivityInfo(typeof(WriteMessageActivity))
      {
        Description = "Output: first <= second"
      };

      outputWhenFalse.AddBinding(new PropertyBindingInfo("Message", PropertyBindingKind.Expression)
      {
        BindingExpression = "() => $\"{first.Get()} <= {second.Get()}\""
      });

      scheme.IntialNode = inputFirst;

      inputFirst.PointsTo = inputSecond;
      inputSecond.PointsTo = condition;

      condition.WhenTrue = outputWhenTrue;
      condition.WhenFalse = outputWhenFalse;

      scheme.AddNodes(inputFirst, inputSecond, condition, outputWhenTrue, outputWhenFalse);

      scheme.AddProperty(new FlowPropertyInfo(typeof(IReader), "Reader"));
      scheme.AddProperty(new FlowPropertyInfo(typeof(IWriter), "Writer"));

      scheme.AddService(
        new ServiceInfo(typeof(IReader), lifetimeKind: LifetimeKind.Singleton, instanceExpression: "Reader"));

      scheme.AddService(
        new ServiceInfo(typeof(IWriter), lifetimeKind: LifetimeKind.Singleton, instanceExpression: "Writer"));

      

      var assembly = new FlowAssemblyEmitter().EmitAssembly(scheme, "Flow1.dll");
      var flowType = assembly.GetType(scheme.FlowFullTypeName);

      return (Flow)Activator.CreateInstance(flowType);
    }    
  }
}