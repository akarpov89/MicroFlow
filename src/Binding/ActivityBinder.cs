using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public class ActivityBinder<TActivity, TProperty> where TActivity : class, IActivity
  {
    private readonly ActivityDescriptor<TActivity> myActivityDescriptor;
    private readonly string myPropertyName;
    private readonly Action<TActivity, TProperty> myPropertySetter;

    internal ActivityBinder(
      [NotNull] ActivityDescriptor<TActivity> activityDescriptor,
      [NotNull] Expression<Func<TActivity, TProperty>> propertyExpression)
    {
      propertyExpression.IsValid().AssertTrue("Invalid property expression");

      myActivityDescriptor = activityDescriptor;
      myPropertyName = propertyExpression.GetPropertyName();
      myPropertySetter = propertyExpression.ConvertToSetter();
    }

    public void To(TProperty value)
    {
      var bindingInfo = new ValueBinding<TProperty>(myPropertyName, value);
      myActivityDescriptor.AddBindingInfo(bindingInfo);

      myActivityDescriptor.AddInitializer(a => SetProperty(a, value));
    }

    public void To([NotNull] Expression<Func<TProperty>> expression)
    {
      expression.AssertNotNull("expression != null");

      var bindingInfo = new ExpressionBinding<TProperty>(myPropertyName, expression);
      myActivityDescriptor.AddBindingInfo(bindingInfo);

      var func = expression.Compile();
      myActivityDescriptor.AddInitializer(a => SetProperty(a, func()));
    }

    public void ToResultOf<TAnotherActivity>([NotNull] ActivityNode<TAnotherActivity> activity)
      where TAnotherActivity : class, IActivity<TProperty>
    {
      activity.AssertNotNull("activity != null");

      ToResultOf(activity.Descriptor);
    }

    public void ToResultOf<TAnotherActivity>([NotNull] ActivityDescriptor<TAnotherActivity> descriptor)
      where TAnotherActivity : class, IActivity<TProperty>
    {
      descriptor.AssertNotNull("descriptor != null");

      var bindingInfo = new ResultBinding<TProperty, TAnotherActivity>(myPropertyName, descriptor);
      myActivityDescriptor.AddBindingInfo(bindingInfo);

      ActivityTaskHandler handler = task =>
      {
        if (task.Status == TaskStatus.RanToCompletion)
        {
          myActivityDescriptor.AddInitializer(a => SetProperty(a, task.Result));
        }
      };

      descriptor.RegisterActivityTaskHandler(handler);
    }

    public void ToExceptionOf([NotNull] IActivityNode activity)
    {
      activity.AssertNotNull("activity != null");

      var bindingInfo = new FaultBinding(myPropertyName, activity);
      myActivityDescriptor.AddBindingInfo(bindingInfo);

      ActivityTaskHandler handler = task =>
      {
        if (task.Status == TaskStatus.Faulted)
        {
          myActivityDescriptor.AddInitializer(a => SetProperty(a, task.Exception));
        }
      };

      activity.RegisterActivityTaskHandler(handler);
    }

    private void SetProperty([NotNull] TActivity activity, object value)
    {
      activity.AssertNotNull("activity != null");
      myPropertySetter(activity, (TProperty) value);
    }
  }
}