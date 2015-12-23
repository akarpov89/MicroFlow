using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MicroFlow
{
    public abstract class Activity<TResult> : IActivity<TResult>
    {
        Task<object> IActivity.Execute()
        {
            return Execute().Convert<TResult, object>();
        }

        public Task<TResult> Execute()
        {
            return ExecuteCore();
        }

        [NotNull]
        protected abstract Task<TResult> ExecuteCore();
    }
}