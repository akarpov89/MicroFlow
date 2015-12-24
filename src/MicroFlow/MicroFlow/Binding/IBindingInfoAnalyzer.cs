using JetBrains.Annotations;

namespace MicroFlow
{
    public interface IBindingInfoAnalyzer<out TResult>
    {
        TResult AnalyzeValueBinding<TProperty>([NotNull] ValueBinding<TProperty> valueBinding);

        TResult AnalyzeExpressionBinding<TProperty>([NotNull] ExpressionBinding<TProperty> expressionBinding);

        TResult AnalyzeResultBinding<TProperty, TActivity>([NotNull] ResultBinding<TProperty, TActivity> resultBinding)
            where TActivity : class, IActivity<TProperty>;

        TResult AnalyzeFaultBinding([NotNull] FaultBinding faultBinding);
    }
}