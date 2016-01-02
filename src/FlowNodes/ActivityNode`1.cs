using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class ActivityNode<TActivity> : ActivityNode where TActivity : class, IActivity
    {
        private readonly ActivityDescriptor<TActivity> _activityDescriptor;

        internal ActivityNode()
        {
            _activityDescriptor = new ActivityDescriptor<TActivity>(Id);
        }

        [NotNull]
        public ActivityDescriptor<TActivity> Descriptor
        {
            get { return _activityDescriptor; }
        }

        public override FlowNodeKind Kind
        {
            get { return FlowNodeKind.Activity; }
        }

        public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
        {
            return visitor.NotNull().VisitActivity(this);
        }

        public override void RemoveConnections()
        {
            base.RemoveConnections();
            _activityDescriptor.Clear();
        }

        public override void RegisterActivityTaskHandler(ActivityTaskHandler handler)
        {
            handler.AssertNotNull("handler != null");
            _activityDescriptor.RegisterActivityTaskHandler(handler);
        }

        public ActivityBinder<TActivity, TProperty> Bind<TProperty>(
            [NotNull] Expression<Func<TActivity, TProperty>> propertyExpression)
        {
            propertyExpression.AssertNotNull("propertyExpression != null");
            return _activityDescriptor.Bind(propertyExpression);
        }

        internal void OnActivityCreated([NotNull] TActivity activity)
        {
            activity.AssertNotNull("activity != null");
            _activityDescriptor.ExecuteInitializersFor(activity);
        }

        internal void OnActivityCompleted([NotNull] Task<object> activityTask)
        {
            activityTask.AssertNotNull("activityTask != null");
            ((IActivityDescriptor) _activityDescriptor).ExecuteActivityTaskHandlers(activityTask);
        }
    }
}