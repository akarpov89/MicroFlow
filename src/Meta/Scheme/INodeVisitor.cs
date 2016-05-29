using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public interface INodeVisitor
  {
    void Visit([NotNull] ActivityInfo node);
    void Visit([NotNull] ConditionInfo node);
    void Visit([NotNull] SwitchInfo node);
    void Visit([NotNull] ForkJoinInfo node);
    void Visit([NotNull] BlockInfo node);
  }
}