using System;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class FlowValidator : NodeVisitor
    {
        protected FlowBuilder Flow { get; private set; }

        [NotNull]
        public ValidationResult Result { get; } = new ValidationResult();

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