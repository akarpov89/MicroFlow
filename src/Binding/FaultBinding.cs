using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class FaultBinding : IPropertyBindingInfo
    {
        public FaultBinding(string propertyName, IActivityNode node)
        {
            PropertyName = propertyName.NotNullOrEmpty("propertyName");
            Node = node.NotNull();
        }

        public string PropertyName { get; private set; }

        public PropertyBindingKind Kind => PropertyBindingKind.Fault;

        public TResult Analyze<TResult>(IBindingInfoAnalyzer<TResult> analyzer)
        {
            return analyzer.NotNull().AnalyzeFaultBinding(this);
        }

        [NotNull]
        public IActivityNode Node { get; private set; }
    }
}