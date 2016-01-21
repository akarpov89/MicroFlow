using System.Threading;
using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class BackgroundActivity<TResult> : Activity<TResult>
    {
        public TaskScheduler Scheduler { get; set; }
        public bool IsLongRunning { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public override Task<TResult> Execute()
        {
            return Task.Factory.StartNew(
                ExecuteCore,
                CancellationToken, IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None,
                Scheduler ?? TaskScheduler.Default);
        }

        protected abstract TResult ExecuteCore();
    }

    public abstract class BackgroundActivity : Activity
    {
        public TaskScheduler Scheduler { get; set; }
        public bool IsLongRunning { get; set; }
        public CancellationToken CancellationToken { get; set; }

        protected override Task ExecuteCore()
        {
            return Task.Factory.StartNew(
                ExecuteActivity,
                CancellationToken, IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None,
                Scheduler ?? TaskScheduler.Current);
        }

        protected abstract void ExecuteActivity();
    }
}