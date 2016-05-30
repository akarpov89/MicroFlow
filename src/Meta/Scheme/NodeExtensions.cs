using JetBrains.Annotations;

namespace MicroFlow.Meta
{
  public static class NodeExtensions
  {
    [NotNull]
    public static TNode WithDescription<TNode>(this TNode node, [NotNull] string description)
      where TNode : NodeInfo
    {
      node.Description = description.NotNull();
      return node;
    }

    [NotNull]
    public static TActivity ConnectTo<TActivity>(this TActivity node, [NotNull] NodeInfo to)
      where TActivity : ActivityLikeNodeInfo
    {
      node.PointsTo = to.NotNull();
      return node;
    }

    [NotNull]
    public static BlockInfo ConnectTo(this BlockInfo node, [NotNull] NodeInfo to)
    {
      node.PointsTo = to.NotNull();
      return node;
    }

    [NotNull]
    public static TActivity ConnectFaultTo<TActivity>(this TActivity node, [NotNull] ActivityInfo to)
      where TActivity : ActivityLikeNodeInfo
    {
      node.FaultHandler = to.NotNull();
      return node;
    }

    [NotNull]
    public static TActivity ConnectCancellationTo<TActivity>(this TActivity node, [NotNull] ActivityInfo to)
      where TActivity : ActivityLikeNodeInfo
    {
      node.CancellationHandler = to.NotNull();
      return node;
    }
  }
}