using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class ActivityBinder<TActivity, TProperty> where TActivity : class, IActivity
    {
        private readonly ActivityDescriptor<TActivity> _activityDescriptor;
        private readonly string _propertyName;
        private readonly Action<TActivity, TProperty> _propertySetter;

        internal ActivityBinder(
            [NotNull] ActivityDescriptor<TActivity> activityDescriptor,
            [NotNull] Expression<Func<TActivity, TProperty>> propertyExpression)
        {
            propertyExpression.IsValid().AssertTrue("Invalid property expression");

            _activityDescriptor = activityDescriptor;
            _propertyName = propertyExpression.GetPropertyName();
            _propertySetter = propertyExpression.ConvertToSetter();
        }

        public void To(TProperty value)
        {
            var bindingInfo = new ValueBinding<TProperty>(_propertyName, value);
            _activityDescriptor.AddBindingInfo(bindingInfo);

            _activityDescriptor.AddInitializer(a => SetProperty(a, value));
        }

        public void To([NotNull] Expression<Func<TProperty>> expression)
        {
            expression.AssertNotNull("expression != null");

            var bindingInfo = new ExpressionBinding<TProperty>(_propertyName, expression);
            _activityDescriptor.AddBindingInfo(bindingInfo);

            var func = expression.Compile();
            _activityDescriptor.AddInitializer(a => SetProperty(a, func()));
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

            var bindingInfo = new ResultBinding<TProperty, TAnotherActivity>(_propertyName, descriptor);
            _activityDescriptor.AddBindingInfo(bindingInfo);

            ActivityTaskHandler handler = task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    _activityDescriptor.AddInitializer(a => SetProperty(a, task.Result));
                }
            };

            descriptor.RegisterActivityTaskHandler(handler);
        }

        public void ToExceptionOf([NotNull] IActivityNode activity)
        {
            activity.AssertNotNull("activity != null");

            var bindingInfo = new FailureBinding(_propertyName, activity);
            _activityDescriptor.AddBindingInfo(bindingInfo);

            ActivityTaskHandler handler = task =>
            {
                if (task.Status == TaskStatus.Faulted)
                {
                    _activityDescriptor.AddInitializer(a => SetProperty(a, task.Exception));
                }
            };

            activity.RegisterActivityTaskHandler(handler);
        }

        private void SetProperty([NotNull] TActivity activity, object value)
        {
            activity.AssertNotNull("activity != null");
            _propertySetter(activity, (TProperty) value);
        }
    }
}