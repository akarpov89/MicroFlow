using System;
using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class SyncActivity<TResult> : Activity<TResult>
    {
        protected sealed override Task<TResult> ExecuteCore()
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                TResult result = ExecuteSyncActivity();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        protected abstract TResult ExecuteSyncActivity();
    }
}