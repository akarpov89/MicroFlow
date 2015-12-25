using System;
using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class SyncActivity<TResult> : Activity<TResult>
    {
        public sealed override Task<TResult> Execute()
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                TResult result = ExecuteActivity();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        protected abstract TResult ExecuteActivity();
    }

    public abstract class SyncActivity : Activity
    {
        protected sealed override Task ExecuteCore()
        {
            var tcs = new TaskCompletionSource<Null>();

            try
            {
                ExecuteActivity();
                tcs.TrySetResult(null);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        protected abstract void ExecuteActivity();
    }
}