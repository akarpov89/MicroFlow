using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace MicroFlow
{
    public static class BuilderExtensions
    {
        public static ThenClause If(
            [NotNull] this FlowBuilder builder,
            [NotNull] string conditionDescription,
            [NotNull] Expression<Func<bool>> condition)
        {
            return new ThenClause(builder, conditionDescription, condition);
        }
    }
}