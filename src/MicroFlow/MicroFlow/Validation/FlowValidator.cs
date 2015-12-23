using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class FlowValidator : UnitVisitor
    {
        protected FlowValidator()
        {
            Result = new ValidationResult();
        }

        protected FlowBuilder Flow { get; private set; }

        [NotNull]
        public ValidationResult Result { get; private set; }

        public bool Validate([NotNull] FlowBuilder flowBuilder)
        {
            Flow = flowBuilder.NotNull();
            Result.ClearErrors();

            foreach (IFlowNode node in flowBuilder.Nodes)
            {
                node.Accept(this);
            }

            PerformGlobalValidation(flowBuilder);

            Flow = null;

            return !Result.HasErrors;
        }

        protected virtual void PerformGlobalValidation([NotNull] FlowBuilder flowBuilder)
        {
        }
    }
}