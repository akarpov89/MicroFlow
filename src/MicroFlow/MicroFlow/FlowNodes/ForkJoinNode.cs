using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public class ForkJoinNode : ActivityNode
    {
        private readonly List<IActivityDescriptor> _forks = new List<IActivityDescriptor>();
        private List<ActivityTaskHandler> _taskHandlers;

        internal ForkJoinNode()
        {
        }

        public override FlowNodeKind Kind
        {
            get { return FlowNodeKind.ForkJoin; }
        }

        [NotNull]
        public ReadOnlyCollection<IActivityDescriptor> Forks
        {
            get { return _forks.AsReadOnly(); }
        }

        public override TResult Accept<TResult>(INodeVisitor<TResult> visitor)
        {
            return visitor.VisitForkJoin(this);
        }

        public override void RemoveConnections()
        {
            base.RemoveConnections();

            foreach (var fork in _forks)
            {
                fork.Clear();
            }

            _forks.Clear();

            if (_taskHandlers != null)
            {
                _taskHandlers.Clear();
            }
        }

        public override void RegisterActivityTaskHandler(ActivityTaskHandler handler)
        {
            handler.AssertNotNull("handler != null");

            if (_taskHandlers == null)
            {
                _taskHandlers = new List<ActivityTaskHandler>();
            }

            _taskHandlers.Add(handler);
        }

        [NotNull]
        public ActivityDescriptor<TActivity> Fork<TActivity>() where TActivity : class, IActivity
        {
            var activityDescriptor = new ActivityDescriptor<TActivity>();
            _forks.Add(activityDescriptor);
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
                _forks[i].ExecuteActivityTaskHandlers(forkTasks[i]);
            }

            _taskHandlers.ExecuteTaskHandlers(joinTask.Convert<object[], object>());
        }
    }
}