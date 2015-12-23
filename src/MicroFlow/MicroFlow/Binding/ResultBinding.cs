using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class ResultBinding<TProperty, TActivity> : IPropertyBindingInfo
        where TActivity : class, IActivity<TProperty>
    {
        public ResultBinding([NotNull] string propertyName, [NotNull] ActivityDescriptor<TActivity> activity)
        {
            PropertyName = propertyName.NotNullOrEmpty("propertyName");
            Activity = activity.NotNull();
        }

        public string PropertyName { get; private set; }

        public PropertyBindingKind Kind
        {
            get { return PropertyBindingKind.Result; }
        }

        public TResult Analyze<TResult>(IBindingInfoAnalyzer<TResult> analyzer)
        {
            return analyzer.NotNull().AnalyzeResultBinding(this);
        }

        [NotNull]
        public ActivityDescriptor<TActivity> Activity { get; private set; }
    }
}