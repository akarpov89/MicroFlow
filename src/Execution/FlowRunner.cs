using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
  public sealed class FlowRunner : INodeVisitor<Task>, IDisposable
  {
    private bool myIsDisposed;
    private ILogger myLog;
    private ServiceProvider myServiceProvider;

    private readonly TaskScheduler myScheduler;

    public FlowRunner()
    {
      var syncContext = SynchronizationContext.Current;

      myScheduler = syncContext != null
        ? TaskScheduler.FromCurrentSynchronizationContext()
        : new CurrentThreadTaskScheduler();
    }

    [NotNull]
    private ILogger Log
    {
      get { return myLog ?? (myLog = new NullLogger()); }
    }

    [NotNull]
    private IServiceProvider ServiceProvider
    {
      get { return myServiceProvider ?? (myServiceProvider = new ServiceProvider(new ServiceCollection())); }
    }

    public void Dispose()
    {
      if (!myIsDisposed)
      {
        myServiceProvider?.Dispose();

        myIsDisposed = true;
      }
    }

    public Task VisitActivity<TActivity>(ActivityNode<TActivity> activityNode) where TActivity : class, IActivity
    {
      activityNode.AssertNotNull("activityNode != null");

      InjectableObject<TActivity> activityWrapper = InjectableObject<TActivity>.Create(ServiceProvider);

      Log.Info("At node: {0}. Activity created: {1}", activityNode, activityWrapper.Instance);

      try
      {
        TActivity activity = activityWrapper.Instance;

        activityNode.OnActivityCreated(activity);

        Log.Info("At node: {0}. Activity started", activityNode);

        Task<object> task = activity.Execute();

        if (task.IsCompleted)
        {
          activityNode.OnActivityCompleted(task);

          activityWrapper.Dispose();

          return ExecuteNextNode(activityNode, task);
        }

        Task<Task> continuation = task.ContinueWith(t =>
        {
          activityNode.OnActivityCompleted(task);

          // ReSharper disable once AccessToDisposedClosure
          activityWrapper.Dispose();

          return ExecuteNextNode(activityNode, t);
        }, myScheduler);

        return continuation.Unwrap();
      }
      catch (Exception)
      {
        activityWrapper.Dispose();
        throw;
      }
    }

    public Task VisitSwitch<TChoice>(SwitchNode<TChoice> switchNode)
    {
      switchNode.AssertNotNull("switchNode != null");

      Log.Info("At node: {0}. Switch entered", switchNode);

      TChoice choice = switchNode.EvaluateChoice();

      Log.Info("At node: {0}. Switch choice evaluated to: '{1}'", switchNode, choice);

      IFlowNode branch = switchNode.Select(choice);
      return branch.Accept(this);
    }

    public Task VisitCondition(ConditionNode conditionNode)
    {
      conditionNode.AssertNotNull("conditionNode != null");

      Log.Info("At node: {0}. Condition entered", conditionNode);

      bool condition = conditionNode.EvaluateCondition();

      Log.Info("At node: {0}. Condition evaluated to: '{1}'", conditionNode, condition);

      IFlowNode branch = condition ? conditionNode.WhenTrue : conditionNode.WhenFalse;

      if (branch == null) return TaskHelper.CompletedTask;

      return branch.Accept(this);
    }

    public Task VisitForkJoin(ForkJoinNode forkJoinNode)
    {
      forkJoinNode.AssertNotNull("forkJoinNode != null");

      var activities = new InjectableObjectsCollection<IActivity>(forkJoinNode.Forks.Count);

      try
      {
        foreach (IActivityDescriptor fork in forkJoinNode.Forks)
        {
          IActivityDescriptor temp = fork;
          IActivity activity = activities.Add(temp.ActivityType, ServiceProvider);
          temp.ExecuteInitializersFor(activity);

          Log.Info("At node: {0}. Fork created: {1}", forkJoinNode, activity);
        }

        Task<object>[] tasks = activities.ConvertInstances(a => a.Execute());
        Task<object[]> task = tasks.WhenAll();

        Log.Info("At node: {0}. Forks started", forkJoinNode);

        Task<Task> continuation = task.ContinueWith(t =>
        {
          forkJoinNode.OnForksJoined(tasks, task);

          // ReSharper disable once AccessToDisposedClosure
          activities.Dispose();

          return ExecuteNextNode(forkJoinNode, t);
        }, myScheduler);

        return continuation.Unwrap();
      }
      catch (Exception)
      {
        activities.Dispose();
        throw;
      }
    }

    public Task VisitBlock(BlockNode blockNode)
    {
      blockNode.AssertNotNull("blockNode != null");

      Log.Info("At node: {0}. Block entered", blockNode);

      if (blockNode.InnerNodes.Count == 0)
      {
        if (blockNode.PointsTo != null)
        {
          return blockNode.PointsTo.Accept(this);
        }

        return TaskHelper.CompletedTask;
      }

      Task innerTask = blockNode.InnerNodes[0].Accept(this);
      Debug.Assert(innerTask != null);

      if (blockNode.PointsTo == null) return innerTask;

      Task<Task> pointsToContinuation = innerTask.ContinueWith(t =>
      {
        if (t.Status == TaskStatus.RanToCompletion)
        {
          return blockNode.PointsTo.Accept(this);
        }

        return t;
      }, myScheduler);

      return pointsToContinuation.Unwrap();
    }

    public Task Run([NotNull] FlowDescription flowDescription)
    {
      flowDescription.AssertNotNull("flowDescription != null");
      flowDescription.InitialNode.AssertNotNull("Initial node isn't set");

      return flowDescription.InitialNode.Accept(this);
    }

    [NotNull]
    public FlowRunner WithServices([NotNull] IServiceCollection services)
    {
      myServiceProvider = new ServiceProvider(services.NotNull());
      return this;
    }

    [NotNull]
    public FlowRunner WithLogger([NotNull] ILogger logger)
    {
      myLog = logger.NotNull();
      return this;
    }

    private Task ExecuteNextNode([NotNull] IActivityNode node, [NotNull] Task task)
    {
      if (task.IsCanceled)
      {
        Log.Info("At node: {0}. Cancelled", node);

        IFlowNode handlerNode = node.CancellationHandler;
        Debug.Assert(handlerNode != null);

        return handlerNode.Accept(this);
      }

      if (task.IsFaulted)
      {
        string message = $"At node: {node}. Faulted";
        Log.Exception(message, task.Exception);

        IFaultHandlerNode handlerNode = node.FaultHandler;
        Debug.Assert(handlerNode != null);

        return handlerNode.Accept(this);
      }

      Log.Info("At node: {0}. Completed", node);

      if (node.PointsTo != null)
      {
        return node.PointsTo.Accept(this);
      }

      return TaskHelper.CompletedTask;
    }
  }
}