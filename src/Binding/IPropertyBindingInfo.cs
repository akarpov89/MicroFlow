using JetBrains.Annotations;

namespace MicroFlow
{
  public interface IPropertyBindingInfo
  {
    [NotNull]
    string PropertyName { get; }

    PropertyBindingKind Kind { get; }

    TResult Analyze<TResult>([NotNull] IBindingInfoAnalyzer<TResult> analyzer);
  }
}