using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public interface IActivityDescriptor
  {
    Guid Id { get; }

    [CanBeNull]
    string Name { get; }

    [NotNull]
    Type ActivityType { get; }

    [NotNull]
    ReadOnlyCollection<IPropertyBindingInfo> PropertyBindings { get; }

    void ExecuteInitializersFor([NotNull] IActivity activity);
    void ExecuteActivityTaskHandlers([NotNull] Task<object> activityTask);

    void Clear();
  }

  public sealed class ActivityDescriptor<TActivity> : IActivityDescriptor
    where TActivity : class, IActivity
  {
    [CanBeNull] private List<Action<TActivity>> myInitializerActions;

    [CanBeNull] private List<ActivityTaskHandler> myTaskHandlers;

    [NotNull] private List<IPropertyBindingInfo> myPropertyBindings = new List<IPropertyBindingInfo>();

    internal ActivityDescriptor() : this(Guid.NewGuid())
    {
    }

    internal ActivityDescriptor(Guid id)
    {
      Id = id;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    Type IActivityDescriptor.ActivityType => typeof (TActivity);

    public ReadOnlyCollection<IPropertyBindingInfo> PropertyBindings
      => new ReadOnlyCollection<IPropertyBindingInfo>(myPropertyBindings);

    void IActivityDescriptor.ExecuteInitializersFor(IActivity activity)
    {
      activity.AssertNotNull("activity != null");
      ExecuteInitializersFor((TActivity) activity);
    }

    void IActivityDescriptor.ExecuteActivityTaskHandlers(Task<object> activityTask)
    {
      myTaskHandlers.ExecuteTaskHandlers(activityTask);
    }

    public void Clear()
    {
      myPropertyBindings.Clear();
      UnregisterAllHandlers();
    }

    public ActivityDescriptor<TActivity> WithName([NotNull] string name)
    {
      Name = name.NotNullOrEmpty("name");
      return this;
    }

    internal void AddInitializer([NotNull] Action<TActivity> action)
    {
      action.AssertNotNull("action != null");

      if (myInitializerActions == null)
      {
        myInitializerActions = new List<Action<TActivity>>();
      }

      myInitializerActions.Add(action);
    }

    internal void AddBindingInfo([NotNull] IPropertyBindingInfo propertyBinding)
    {
      propertyBinding.AssertNotNull("propertyBinding != null");
      myPropertyBindings.Add(propertyBinding);
    }

    public ActivityBinder<TActivity, TProperty> Bind<TProperty>(
      [NotNull] Expression<Func<TActivity, TProperty>> propertyExpression)
    {
      propertyExpression.AssertNotNull("propertyExpression != null");
      return new ActivityBinder<TActivity, TProperty>(this, propertyExpression);
    }

    internal void ExecuteInitializersFor([NotNull] TActivity activity)
    {
      activity.AssertNotNull("activity != null");

      if (myInitializerActions != null)
      {
        foreach (Action<TActivity> action in myInitializerActions)
        {
          action(activity);
        }
      }
    }

    internal void RegisterActivityTaskHandler([NotNull] ActivityTaskHandler handler)
    {
      handler.AssertNotNull("handler != null");

      if (myTaskHandlers == null)
      {
        myTaskHandlers = new List<ActivityTaskHandler>();
      }

      myTaskHandlers.Add(handler);
    }

    internal void UnregisterAllHandlers()
    {
      myInitializerActions?.Clear();
      myTaskHandlers?.Clear();
    }
  }
}