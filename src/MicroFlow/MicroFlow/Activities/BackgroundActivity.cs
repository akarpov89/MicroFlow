using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class BackgroundActivity<TResult> : Activity<TResult>
    {
        [CanBeNull]
        public TaskScheduler Scheduler { get; set; }

        public bool IsLongRunning { get; set; }

        protected override Task<TResult> ExecuteCore()
        {
            if (Scheduler != null)
            {
                return Task.Factory.StartNew(
                    () => ExecuteSyncAction(),
                    CancellationToken.None, IsLongRunning ? TaskCreationOptions.LongRunning : TaskCreationOptions.None,
                    Scheduler);
            }

            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                tcs.TrySetResult(ExecuteSyncAction());
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        protected abstract TResult ExecuteSyncAction();
    }
}