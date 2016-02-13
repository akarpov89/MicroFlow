using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace MicroFlow
{
  public class ExpressionAnalyzer : ExpressionVisitor
  {
    private readonly List<Guid> myDependencies = new List<Guid>();

    public ReadOnlyCollection<Guid> Dependencies => new ReadOnlyCollection<Guid>(myDependencies);

    public bool IsValid { get; private set; } = true;

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      if (node.Method.Name == "Get" && node.Object != null)
      {
        if (node.Object.Type.Is<IResult>())
        {
          if (node.Object.NodeType != ExpressionType.MemberAccess)
          {
            IsValid = false;
          }
          else
          {
            var memberExpression = (MemberExpression) node.Object;

            if (memberExpression.Expression.NodeType != ExpressionType.Constant)
            {
              IsValid = false;
            }
            else
            {
              var constantExpression = (ConstantExpression) memberExpression.Expression;
              var closure = constantExpression.Value;

              var resultField = (FieldInfo) memberExpression.Member;
              var result = (IResult) resultField.GetValue(closure);

              myDependencies.Add(result.SourceId);
            }
          }
        }
      }

      return base.VisitMethodCall(node);
    }
  }
}