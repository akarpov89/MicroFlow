using System.Threading;
using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class BackgroundActivity<TResult> : Activity<TResult>
    {
        private CancellationTokenSource _cts;

        public TaskScheduler Scheduler { get; set; }
        public bool IsLongRunning { get; set; }

        public override Task<TResult> Execute()
        {
            _cts = new CancellationTokenSource();

            return Task.Factory.StartNew(
                () => ExecuteCore(_cts.Token),
                _cts.Token, IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None,
                Scheduler ?? TaskScheduler.Default);
        }

        public void Cancel()
        {
            _cts.AssertNotNull("Activity isn't started");
            _cts.Cancel();
        }

        protected abstract TResult ExecuteCore(CancellationToken token);
    }

    public abstract class BackgroundActivity : Activity
    {
        private CancellationTokenSource _cts;

        public TaskScheduler Scheduler { get; set; }
        public bool IsLongRunning { get; set; }

        protected override Task ExecuteCore()
        {
            _cts = new CancellationTokenSource();

            return Task.Factory.StartNew(
                () => ExecuteActivity(_cts.Token),
                _cts.Token, IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None,
                Scheduler ?? TaskScheduler.Current);
        }

        public void Cancel()
        {
            _cts.AssertNotNull("Activity isn't started");
            _cts.Cancel();
        }

        protected abstract void ExecuteActivity(CancellationToken token);
    }
}