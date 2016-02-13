using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public static class VariableBindingExtensions
  {
    public static void BindToResultOf<TVariable, TActivity>(
      [NotNull] this Variable<TVariable> variable, [NotNull] ActivityNode<TActivity> node)
      where TActivity : class, IActivity<TVariable>
    {
      variable.AssertNotNull("variable != null");
      node.AssertNotNull("node != null");

      BindToResultOf(variable, node.Descriptor);
    }

    public static void BindToResultOf<TVariable, TActivity>(
      [NotNull] this Variable<TVariable> variable, [NotNull] ActivityDescriptor<TActivity> descriptor)
      where TActivity : class, IActivity<TVariable>
    {
      variable.AssertNotNull("variable != null");
      descriptor.AssertNotNull("descriptor != null");

      descriptor.RegisterActivityTaskHandler(t =>
      {
        if (t.Status == TaskStatus.RanToCompletion)
        {
          variable.Assign((TVariable) t.Result);
        }
      });
    }

    public static void OnCompletionAssign<TActivity, TVariable>(
      [NotNull] this ActivityNode<TActivity> node, [NotNull] Variable<TVariable> variable, TVariable value)
      where TActivity : class, IActivity
    {
      node.AssertNotNull("node != null");
      variable.AssertNotNull("variable != null");

      OnCompletionAssign(node.Descriptor, variable, value);
    }

    public static void OnCompletionAssign<TActivity, TVariable>(
      [NotNull] this ActivityDescriptor<TActivity> descriptor, [NotNull] Variable<TVariable> variable, TVariable value)
      where TActivity : class, IActivity
    {
      descriptor.AssertNotNull("descriptor != null");
      variable.AssertNotNull("variable != null");

      descriptor.RegisterActivityTaskHandler(t =>
      {
        if (t.Status == TaskStatus.RanToCompletion)
        {
          variable.Assign(value);
        }
      });
    }

    public static void OnCompletionUpdate<TActivity, TVariable>(
      [NotNull] this ActivityNode<TActivity> node, [NotNull] Variable<TVariable> variable,
      [NotNull] Func<TVariable, TVariable> updateFunc)
      where TActivity : class, IActivity
    {
      node.AssertNotNull("node != null");
      variable.AssertNotNull("variable != null");
      updateFunc.AssertNotNull("updateFunc != null");

      OnCompletionUpdate(node.Descriptor, variable, updateFunc);
    }

    public static void OnCompletionUpdate<TActivity, TVariable>(
      [NotNull] this ActivityDescriptor<TActivity> descriptor, [NotNull] Variable<TVariable> variable,
      [NotNull] Func<TVariable, TVariable> updateFunc)
      where TActivity : class, IActivity
    {
      descriptor.AssertNotNull("descriptor != null");
      variable.AssertNotNull("variable != null");

      descriptor.RegisterActivityTaskHandler(t =>
      {
        if (t.Status == TaskStatus.RanToCompletion)
        {
          variable.Update(updateFunc);
        }
      });
    }

    public static void OnCompletionUpdate<TActivity, TActivityResult, TVariable>(
      [NotNull] this ActivityNode<TActivity> node, [NotNull] Variable<TVariable> variable,
      [NotNull] Func<TVariable, TActivityResult, TVariable> updateFunc)
      where TActivity : class, IActivity<TActivityResult>
    {
      node.AssertNotNull("node != null");
      variable.AssertNotNull("variable != null");
      updateFunc.AssertNotNull("updateFunc != null");

      OnCompletionUpdate(node.Descriptor, variable, updateFunc);
    }

    public static void OnCompletionUpdate<TActivity, TActivityResult, TVariable>(
      [NotNull] this ActivityDescriptor<TActivity> descriptor, [NotNull] Variable<TVariable> variable,
      [NotNull] Func<TVariable, TActivityResult, TVariable> updateFunc)
      where TActivity : class, IActivity<TActivityResult>
    {
      descriptor.AssertNotNull("descriptor != null");
      variable.AssertNotNull("variable != null");

      descriptor.RegisterActivityTaskHandler(t =>
      {
        if (t.Status == TaskStatus.RanToCompletion)
        {
          variable.Update(updateFunc, (TActivityResult) t.Result);
        }
      });
    }
  }
}