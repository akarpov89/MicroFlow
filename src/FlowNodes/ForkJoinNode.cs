using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public class ForkJoinNode : ActivityNode
  {
    private readonly List<IActivityDescriptor> myForks = new List<IActivityDescriptor>();
    private List<ActivityTaskHandler> myTaskHandlers;

    internal ForkJoinNode()
    {
    }

    public override FlowNodeKind Kind => FlowNodeKind.ForkJoin;

    [NotNull]
    public ReadOnlyCollection<IActivityDescriptor> Forks => new ReadOnlyCollection<IActivityDescriptor>(myForks);

    public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
    {
      return visitor.VisitForkJoin(this);
    }

    public override void RemoveConnections()
    {
      base.RemoveConnections();

      foreach (var fork in myForks)
      {
        fork.Clear();
      }

      myForks.Clear();
      myTaskHandlers?.Clear();
    }

    public override void RegisterActivityTaskHandler(ActivityTaskHandler handler)
    {
      handler.AssertNotNull("handler != null");

      if (myTaskHandlers == null)
      {
        myTaskHandlers = new List<ActivityTaskHandler>();
      }

      myTaskHandlers.Add(handler);
    }

    [NotNull]
    public ActivityDescriptor<TActivity> Fork<TActivity>() where TActivity : class, IActivity
    {
      var activityDescriptor = new ActivityDescriptor<TActivity>();
      myForks.Add(activityDescriptor);
      return activityDescriptor;
    }

    [NotNull]
    public ActivityDescriptor<TActivity> Fork<TActivity>([NotNull] string forkName)
      where TActivity : class, IActivity
    {
      return Fork<TActivity>().WithName(forkName);
    }

    internal void OnForksJoined([NotNull] Task<object>[] forkTasks, [NotNull] Task<object[]> joinTask)
    {
      forkTasks.AssertNotNull("forkTasks != null");
      joinTask.AssertNotNull("joinTask != null");
      (forkTasks.Length == Forks.Count).AssertTrue("Invalid number of tasks");

      for (int i = 0; i < forkTasks.Length; i++)
      {
        myForks[i].ExecuteActivityTaskHandlers(forkTasks[i]);
      }

      myTaskHandlers.ExecuteTaskHandlers(joinTask.Convert<object[], object>());
    }
  }
}