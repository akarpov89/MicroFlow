using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public class ActivityNode<TActivity> : ActivityNode where TActivity : class, IActivity
  {
    private readonly ActivityDescriptor<TActivity> myActivityDescriptor;

    internal ActivityNode()
    {
      myActivityDescriptor = new ActivityDescriptor<TActivity>(Id);
    }

    [NotNull]
    public ActivityDescriptor<TActivity> Descriptor => myActivityDescriptor;

    public override FlowNodeKind Kind => FlowNodeKind.Activity;

    public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
      return visitor.NotNull().VisitActivity(this);
    }

    public override void RemoveConnections()
    {
      base.RemoveConnections();
      myActivityDescriptor.Clear();
    }

    public override void RegisterActivityTaskHandler(ActivityTaskHandler handler)
    {
      handler.AssertNotNull("handler != null");
      myActivityDescriptor.RegisterActivityTaskHandler(handler);
    }

    public ActivityBinder<TActivity, TProperty> Bind<TProperty>(
      [NotNull] Expression<Func<TActivity, TProperty>> propertyExpression)
    {
      propertyExpression.AssertNotNull("propertyExpression != null");
      return myActivityDescriptor.Bind(propertyExpression);
    }

    internal void OnActivityCreated([NotNull] TActivity activity)
    {
      activity.AssertNotNull("activity != null");
      myActivityDescriptor.ExecuteInitializersFor(activity);
    }

    internal void OnActivityCompleted([NotNull] Task<object> activityTask)
    {
      activityTask.AssertNotNull("activityTask != null");
      ((IActivityDescriptor) myActivityDescriptor).ExecuteActivityTaskHandlers(activityTask);
    }
  }
}