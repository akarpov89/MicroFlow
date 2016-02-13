using JetBrains.Annotations;

namespace MicroFlow
{
  public abstract class FlowValidator : NodeVisitor
  {
    protected FlowDescription Flow { get; private set; }

    [NotNull]
    public ValidationResult Result { get; } = new ValidationResult();

    public bool Validate([NotNull] FlowDescription flowDescription)
    {
      Flow = flowDescription.NotNull();
      Result.ClearErrors();

      foreach (IFlowNode node in flowDescription.Nodes)
      {
        node.Accept(this);
      }

      PerformGlobalValidation(flowDescription);

      Flow = null;

      return !Result.HasErrors;
    }

    protected virtual void PerformGlobalValidation([NotNull] FlowDescription flowDescription)
    {
    }
  }
}