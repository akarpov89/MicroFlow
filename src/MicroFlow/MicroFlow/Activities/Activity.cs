using System.Threading.Tasks;

namespace MicroFlow
{
    public abstract class Activity<TResult> : IActivity<TResult>
    {
        Task<object> IActivity.Execute()
        {
            return Execute().Convert<TResult, object>();
        }

        public abstract Task<TResult> Execute();
    }

    public abstract class Activity : Activity<Null>
    {
        public override Task<Null> Execute()
        {
            return ExecuteCore().ToTaskOfNull();
        }

        protected abstract Task ExecuteCore();
    }
}