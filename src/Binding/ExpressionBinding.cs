using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class ExpressionBinding<TProperty> : IPropertyBindingInfo
    {
        internal ExpressionBinding([NotNull] string propertyName, [NotNull] Expression<Func<TProperty>> expression)
        {
            PropertyName = propertyName.NotNullOrEmpty("propertyName");
            Expression = expression.NotNull();
        }

        public string PropertyName { get; private set; }

        public PropertyBindingKind Kind => PropertyBindingKind.Expression;

        public TResult Analyze<TResult>(IBindingInfoAnalyzer<TResult> analyzer)
        {
            return analyzer.NotNull().AnalyzeExpressionBinding(this);
        }

        [NotNull]
        public Expression<Func<TProperty>> Expression { get; private set; }
    }
}