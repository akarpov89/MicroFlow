using System;
using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class SynchronousActivity<TResult> : Activity<TResult>
    {
        protected sealed override Task<TResult> ExecuteCore()
        {
            var tcs = new TaskCompletionSource<TResult>();

            try
            {
                TResult result = ExecuteSynchronously();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }

        protected abstract TResult ExecuteSynchronously();
    }
}