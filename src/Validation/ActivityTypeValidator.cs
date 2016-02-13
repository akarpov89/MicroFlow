using System;
#if PORTABLE
using System.Linq;
#endif
using System.Reflection;
using JetBrains.Annotations;

namespace MicroFlow
{
  public sealed class ActivityTypeValidator : FlowValidator
  {
    protected override void VisitActivity<TActivity>(ActivityNode<TActivity> activityNode)
    {
      CheckActivityType(typeof (TActivity), activityNode);
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
        CheckActivityType(fork.ActivityType, forkJoinNode);
      }
    }

    protected override void VisitBlock(BlockNode blockNode)
    {
    }

    private void CheckActivityType([NotNull] Type activityType, [NotNull] IFlowNode node)
    {
#if PORTABLE
            var typeInfo = activityType.GetTypeInfo();
#else
      var typeInfo = activityType;

#endif
      if (typeInfo.IsAbstract)
      {
        Result.AddError(node, $"Activity {activityType.Name} is abstract and cannot be instanciated");
        return;
      }

      if (typeInfo.IsGenericTypeDefinition)
      {
        Result.AddError(node, $"Activity {activityType.Name} is generic type definition and cannot be instanciated");
        return;
      }
#if PORTABLE
            if (!typeInfo.DeclaredConstructors.Any(c => !c.IsStatic && c.IsPublic))
            {
                Result.AddError(node, $"Activity {activityType.Name} has no public constructors");
            }
#else
      if (typeInfo.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0)
      {
        Result.AddError(node, $"Activity {activityType.Name} has no public constructors");
      }
#endif
    }
  }
}