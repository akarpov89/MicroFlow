using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;

namespace MicroFlow
{
  internal static class ExpressionHelper
  {
    public static bool IsValid<T, TProperty>(
      [NotNull] this Expression<Func<T, TProperty>> propertyExpression)
    {
      propertyExpression.AssertNotNull("propertyExpression != null");

      var param = propertyExpression.Parameters[0];
      var memberExpression = propertyExpression.Body as MemberExpression;

      return memberExpression != null &&
             memberExpression.Expression.Equals(param) &&
             memberExpression.Member is PropertyInfo;
    }

    public static string GetPropertyName<T, TProperty>(
      [NotNull] this Expression<Func<T, TProperty>> propertyExpression)
    {
      propertyExpression.AssertNotNull("propertyExpression != null");

      var memberExpression = (MemberExpression) propertyExpression.Body;
      return memberExpression.Member.Name;
    }

    public static Action<T, TProperty> ConvertToSetter<T, TProperty>(
      [NotNull] this Expression<Func<T, TProperty>> propertyExpression)
    {
      propertyExpression.AssertNotNull("propertyExpression != null");

      var member = (MemberExpression) propertyExpression.Body;
      ParameterExpression param = Expression.Parameter(typeof (TProperty), "value");
      Expression<Action<T, TProperty>> setter =
        Expression.Lambda<Action<T, TProperty>>(Expression.Assign(member, param),
          propertyExpression.Parameters[0], param);

      Action<T, TProperty> compiledSetter = setter.Compile();

      return compiledSetter;
    }
  }
}