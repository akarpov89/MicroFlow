using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class FailureBinding : IPropertyBindingInfo
    {
        public FailureBinding(string propertyName, IActivityNode node)
        {
            PropertyName = propertyName.NotNullOrEmpty("propertyName");
            Node = node.NotNull();
        }

        public string PropertyName { get; private set; }

        public PropertyBindingKind Kind
        {
            get { return PropertyBindingKind.Failure; }
        }

        public TResult Analyze<TResult>(IBindingInfoAnalyzer<TResult> analyzer)
        {
            return analyzer.NotNull().AnalyzeFailureBinding(this);
        }

        [NotNull]
        public IActivityNode Node { get; private set; }
    }
}