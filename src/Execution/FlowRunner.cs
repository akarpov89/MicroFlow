using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public sealed class FlowRunner : INodeVisitor<Task>, IDisposable
    {
        private bool _isDisposed;
        private ILogger _log;
        private ServiceProvider _serviceProvider;

        private readonly TaskScheduler _scheduler;

        public FlowRunner()
        {
            var syncContext = SynchronizationContext.Current;

            _scheduler = syncContext != null
                ? TaskScheduler.FromCurrentSynchronizationContext()
                : new CurrentThreadTaskScheduler();
        }

        [NotNull]
        private ILogger Log
        {
            get { return _log ?? (_log = new NullLogger()); }
        }

        [NotNull]
        private IServiceProvider ServiceProvider
        {
            get { return _serviceProvider ?? (_serviceProvider = new ServiceProvider(new ServiceCollection())); }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _serviceProvider?.Dispose();

                _isDisposed = true;
            }
        }

        public Task VisitActivity<TActivity>(ActivityNode<TActivity> activityNode) where TActivity : class, IActivity
        {
            activityNode.AssertNotNull("activityNode != null");

            InjectableObject<TActivity> activityWrapper = InjectableObject<TActivity>.Create(ServiceProvider);

            Log.Info("At node: {0}{1}Activity created: {2}", activityNode, Environment.NewLine, activityWrapper.Instance);

            try
            {
                TActivity activity = activityWrapper.Instance;

                activityNode.OnActivityCreated(activity);

                Log.Info("At node: {0}{1}Activity started", activityNode, Environment.NewLine);

                Task<object> task = activity.Execute();

                Task<Task> continuation = task.ContinueWith(t =>
                {
                    activityNode.OnActivityCompleted(task);

                    // ReSharper disable once AccessToDisposedClosure
                    activityWrapper.Dispose();

                    return ExecuteNextNode(activityNode, t);
                }, _scheduler);

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

            Log.Info("At node: {0}{1}Switch entered", switchNode, Environment.NewLine);

            TChoice choice = switchNode.EvaluateChoice();

            Log.Info("At node: {0}{1}Switch choice evaluated to: '{2}'", switchNode, Environment.NewLine, choice);

            IFlowNode branch = switchNode.Select(choice);
            return branch.Accept(this);
        }

        public Task VisitCondition(ConditionNode conditionNode)
        {
            conditionNode.AssertNotNull("conditionNode != null");

            Log.Info("At node: {0}{1}Condition entered", conditionNode, Environment.NewLine);

            bool condition = conditionNode.EvaluateCondition();

            Log.Info("At node: {0}{1}Condition evaluated to: '{2}'", conditionNode, Environment.NewLine, condition);

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

                    Log.Info("At node: {0}{1}Fork created: {2}", forkJoinNode, Environment.NewLine, activity);
                }

                Task<object>[] tasks = activities.ConvertInstances(a => a.Execute());
                Task<object[]> task = tasks.WhenAll();

                Log.Info("At node: {0}{1}Forks started", forkJoinNode, Environment.NewLine);

                Task<Task> continuation = task.ContinueWith(t =>
                {
                    forkJoinNode.OnForksJoined(tasks, task);

                    // ReSharper disable once AccessToDisposedClosure
                    activities.Dispose();

                    return ExecuteNextNode(forkJoinNode, t);
                }, _scheduler);

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
            }, _scheduler);

            return pointsToContinuation.Unwrap();
        }

        public Task Run([NotNull] FlowBuilder flowBuilder)
        {
            flowBuilder.AssertNotNull("flowBuilder != null");
            flowBuilder.InitialNode.AssertNotNull("Initial node isn't set");

            return flowBuilder.InitialNode.Accept(this);
        }

        [NotNull]
        public FlowRunner WithServices([NotNull] IServiceCollection services)
        {
            _serviceProvider = new ServiceProvider(services.NotNull());
            return this;
        }

        [NotNull]
        public FlowRunner WithLogger([NotNull] ILogger logger)
        {
            _log = logger.NotNull();
            return this;
        }

        private Task ExecuteNextNode([NotNull] IActivityNode node, [NotNull] Task task)
        {
            if (task.IsCanceled)
            {
                Log.Info("At node: {0}{1}Cancelled", node, Environment.NewLine);

                IFlowNode handlerNode = node.CancellationHandler;
                Debug.Assert(handlerNode != null);

                return handlerNode.Accept(this);
            }

            if (task.IsFaulted)
            {
                string message = $"At node: {node}{Environment.NewLine}Faulted";
                Log.Exception(message, task.Exception);

                IFaultHandlerNode handlerNode = node.FaultHandler;
                Debug.Assert(handlerNode != null);

                return handlerNode.Accept(this);
            }

            Log.Info("At node: {0}{1}Completed", node, Environment.NewLine);

            if (node.PointsTo != null)
            {
                return node.PointsTo.Accept(this);
            }

            return TaskHelper.CompletedTask;
        }
    }
}