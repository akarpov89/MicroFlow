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

        public static FlowBuilder WithDefaultFaultHandler<TFaultHandler>(
            [NotNull] this FlowBuilder builder,
            [NotNull] string faultHandlerName = "Global fault handler") 
            where TFaultHandler : class, IFaultHandlerActivity
        {
            var faultHandler = builder.FaultHandler<TFaultHandler>(faultHandlerName);
            return builder.WithDefaultFaultHandler(faultHandler);
        }

        public static FlowBuilder WithDefaultCancellationHandler<TCancellationHandler>(
            [NotNull] this FlowBuilder builder,
            [NotNull] string cancellationHandlerName = "Global cancellation handler")
            where TCancellationHandler : class, IActivity
        {
            var cancellationHandler = builder.Activity<TCancellationHandler>(cancellationHandlerName);
            return builder.WithDefaultCancellationHandler(cancellationHandler);
        }
    }
}