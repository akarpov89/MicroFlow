using System.Linq;

namespace MicroFlow
{
  public class ActivityInitializationValidator : FlowValidator
  {
    protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
    {
      if (!string.IsNullOrEmpty(activityNode.Name))
      {
        activityNode.Descriptor.WithName(activityNode.Name);
      }

      CheckActivityBindings(activityNode.Descriptor);
    }

    protected override void VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
    {
    }

    protected override void VisitCondition(ConditionNode conditionNode)
    {
    }

    protected override void VisitForkJoin(ForkJoinNode forkJoinNode)
    {
      foreach (var fork in forkJoinNode.Forks)
      {
        CheckActivityBindings(fork);
      }
    }

    protected override void VisitBlock(BlockNode blockNode)
    {
    }

    private void CheckActivityBindings(IActivityDescriptor descriptor)
    {
      CheckForMultipleInitializers(descriptor);
      CheckRequiredInitializers(descriptor);
    }

    private void CheckForMultipleInitializers(IActivityDescriptor descriptor)
    {
      if (descriptor.PropertyBindings.Count == 0) return;

      var bindings = descriptor.PropertyBindings;

      for (int current = 1; current < bindings.Count; ++current)
      {
        var currentProperty = bindings[current].PropertyName;

        for (int beforeCurrent = 0; beforeCurrent < current; ++beforeCurrent)
        {
          var beforeCurrentProperty = bindings[beforeCurrent].PropertyName;

          if (currentProperty == beforeCurrentProperty)
          {
            var message =
              "Multiple initializers of the property " +
              $"{descriptor.ActivityType.Name}.{currentProperty}";

            Result.AddError(descriptor, message);
            break;
          }
        }
      }
    }

    private void CheckRequiredInitializers(IActivityDescriptor descriptor)
    {
      var bindings = descriptor.PropertyBindings;

      var requiredProperties = descriptor.ActivityType.GetRequiredProperties();
      foreach (var requiredProperty in requiredProperties)
      {
        bool hasInitializer = bindings.Any(binding => requiredProperty == binding.PropertyName);

        if (!hasInitializer)
        {
          var message =
            "No initializer of the property " +
            $"{descriptor.ActivityType.Name}.{requiredProperty}";

          Result.AddError(descriptor, message);
        }
      }
    }
  }
}